using System.Security.Authentication;
using Google.Apis.Auth;
using Hakeem.Application.DTOs.Auth;
using Hakeem.Application.Interfaces;
using Hakeem.Domain.Entities;
using Hakeem.Domain.Enums;
using Hakeem.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Hakeem.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IEmailService emailService,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Role = UserRole.Patient
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Registration failed: {errors}");
        }

        await _userManager.AddToRoleAsync(user, UserRole.Patient.ToString());

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        
        var verificationLink = $"https://hakeem.app/verify-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
        
        await _emailService.SendEmailVerificationAsync(user.Email, user.FullName, verificationLink);
    }

    public async Task<AuthResponseDto> VerifyEmailAsync(VerifyEmailRequestDto request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
            throw new InvalidOperationException("Invalid or expired verification token.");

        return await GenerateAuthResponseAsync(user);
    }

    public async Task ResendVerificationAsync(ResendVerificationRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return;
        
        if (user.EmailConfirmed)
            throw new InvalidOperationException("Email is already verified.");

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var verificationLink = $"https://hakeem.app/verify-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
        
        await _emailService.SendEmailVerificationAsync(user.Email!, user.FullName, verificationLink);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            throw new AuthenticationException("Invalid email or password.");

        if (!user.EmailConfirmed)
            throw new AuthenticationException("EMAIL_NOT_VERIFIED");

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> GoogleLoginAsync(GoogleLoginRequestDto request)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
        }
        catch (Exception ex)
        {
            throw new AuthenticationException($"Invalid Google token: {ex.Message}");
        }

        var user = await _userManager.FindByEmailAsync(payload.Email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = payload.Email,
                Email = payload.Email,
                FullName = payload.Name,
                EmailConfirmed = true,
                Role = UserRole.Patient
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                throw new InvalidOperationException("Failed to create user from Google login.");

            await _userManager.AddToRoleAsync(user, UserRole.Patient.ToString());
        }

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var repo = _unitOfWork.Repository<RefreshToken>();
        var tokens = await repo.GetAllAsync();
        var refreshToken = tokens.FirstOrDefault(t => t.Token == request.RefreshToken);

        if (refreshToken == null || !refreshToken.IsActive)
            throw new AuthenticationException("Invalid or expired refresh token.");

        refreshToken.RevokedAt = DateTime.UtcNow;
        repo.Update(refreshToken);
        
        var user = await _userManager.FindByIdAsync(refreshToken.UserId);
        if (user == null)
            throw new AuthenticationException("User not found.");

        var response = await GenerateAuthResponseAsync(user);
        
        var newTokens = await repo.GetAllAsync();
        var newRefreshToken = newTokens.FirstOrDefault(t => t.Token == response.RefreshToken && t.UserId == user.Id && t.IsActive);
        if (newRefreshToken != null)
        {
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            repo.Update(refreshToken);
        }
        
        await _unitOfWork.SaveChangesAsync();

        return response;
    }

    public async Task LogoutAsync(string userId, string refreshToken)
    {
        var repo = _unitOfWork.Repository<RefreshToken>();
        var tokens = await repo.GetAllAsync();
        var token = tokens.FirstOrDefault(t => t.Token == refreshToken && t.UserId == userId && t.IsActive);
        
        if (token != null)
        {
            token.RevokedAt = DateTime.UtcNow;
            repo.Update(token);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshTokenString = _tokenService.GenerateRefreshToken();
        
        var refreshToken = new RefreshToken
        {
            Token = refreshTokenString,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            Role = roles.FirstOrDefault() ?? user.Role.ToString()
        };
    }
}
