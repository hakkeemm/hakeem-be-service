using Google.Apis.Auth;
using Hakeem.Application.DTOs.Auth;
using Hakeem.Application.Interfaces;
using Hakeem.Domain.Common;
using Hakeem.Domain.Entities;
using Hakeem.Domain.Enums;
using Hakeem.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace Hakeem.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IEmailService emailService,
        IUnitOfWork unitOfWork,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<Result> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
            return Result.Failure(new Error("Auth.UserExists", "User with this email already exists."));

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
            return Result.Failure(new Error("Auth.RegistrationFailed", $"Registration failed: {errors}"));
        }

        await _userManager.AddToRoleAsync(user, UserRole.Patient.ToString());

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var verificationLink = $"https://hakeem.app/verify-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
        
        await _emailService.SendEmailVerificationAsync(user.Email, user.FullName, verificationLink);

        return Result.Success();
    }

    public async Task<Result<AuthResponseDto>> VerifyEmailAsync(VerifyEmailRequestDto request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return Result.Failure<AuthResponseDto>(new Error("Auth.UserNotFound", "User not found."));

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
            return Result.Failure<AuthResponseDto>(new Error("Auth.InvalidToken", "Invalid or expired verification token."));

        return Result.Success(await GenerateAuthResponseAsync(user));
    }

    public async Task<Result> ResendVerificationAsync(ResendVerificationRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) 
            return Result.Success();
        
        if (user.EmailConfirmed)
            return Result.Failure(new Error("Auth.AlreadyVerified", "Email is already verified."));

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var verificationLink = $"https://hakeem.app/verify-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
        
        await _emailService.SendEmailVerificationAsync(user.Email!, user.FullName, verificationLink);
        
        return Result.Success();
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Result.Failure<AuthResponseDto>(new Error("Auth.InvalidCredentials", "Invalid email or password."));

        if (!user.EmailConfirmed)
            return Result.Failure<AuthResponseDto>(new Error("Auth.EmailNotVerified", "Email is not verified."));

        return Result.Success(await GenerateAuthResponseAsync(user));
    }

    public async Task<Result<AuthResponseDto>> GoogleLoginAsync(GoogleLoginRequestDto request)
    {
        GoogleJsonWebSignature.Payload payload;
        try
        {
            var clientId = _configuration["Authentication:Google:ClientId"];
            var settings = new GoogleJsonWebSignature.ValidationSettings();
            if (!string.IsNullOrEmpty(clientId))
            {
                settings.Audience = new[] { clientId };
            }
            
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
        }
        catch (Exception ex)
        {
            return Result.Failure<AuthResponseDto>(new Error("Auth.InvalidGoogleToken", $"Invalid Google token: {ex.Message}"));
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
                return Result.Failure<AuthResponseDto>(new Error("Auth.GoogleLoginFailed", "Failed to create user from Google login."));

            await _userManager.AddToRoleAsync(user, UserRole.Patient.ToString());
        }

        return Result.Success(await GenerateAuthResponseAsync(user));
    }

    public async Task<Result<AuthResponseDto>> GoogleCallbackAsync(string code)
    {
        var clientId = _configuration["Authentication:Google:ClientId"];
        var clientSecret = _configuration["Authentication:Google:ClientSecret"];
        var callbackUrl = _configuration["Authentication:Google:CallbackUrl"];

        using var client = new HttpClient();
        var requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"code", code},
            {"client_id", clientId ?? ""},
            {"client_secret", clientSecret ?? ""},
            {"redirect_uri", callbackUrl ?? ""},
            {"grant_type", "authorization_code"}
        });

        var response = await client.PostAsync("https://oauth2.googleapis.com/token", requestContent);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return Result.Failure<AuthResponseDto>(new Error("Auth.GoogleTokenExchangeFailed", $"Failed to exchange code for token."));
        }

        var json = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonDocument.Parse(json);
        if (!tokenResponse.RootElement.TryGetProperty("id_token", out var idTokenElement))
        {
            return Result.Failure<AuthResponseDto>(new Error("Auth.GoogleNoIdToken", "No id_token received from Google."));
        }

        var idToken = idTokenElement.GetString();
        return await GoogleLoginAsync(new GoogleLoginRequestDto { IdToken = idToken! });
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var repo = _unitOfWork.Repository<RefreshToken>();
        var tokens = await repo.GetAllAsync();
        var refreshToken = tokens.FirstOrDefault(t => t.Token == request.RefreshToken);

        if (refreshToken == null || !refreshToken.IsActive)
            return Result.Failure<AuthResponseDto>(new Error("Auth.InvalidRefreshToken", "Invalid or expired refresh token."));

        refreshToken.RevokedAt = DateTime.UtcNow;
        repo.Update(refreshToken);
        
        var user = await _userManager.FindByIdAsync(refreshToken.UserId);
        if (user == null)
            return Result.Failure<AuthResponseDto>(new Error("Auth.UserNotFound", "User not found."));

        var response = await GenerateAuthResponseAsync(user);
        
        var newTokens = await repo.GetAllAsync();
        var newRefreshToken = newTokens.FirstOrDefault(t => t.Token == response.RefreshToken && t.UserId == user.Id && t.IsActive);
        if (newRefreshToken != null)
        {
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            repo.Update(refreshToken);
        }
        
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(response);
    }

    public async Task<Result> LogoutAsync(string userId, string refreshToken)
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
        return Result.Success();
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
