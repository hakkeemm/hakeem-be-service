using Hakeem.Application.DTOs.Auth;
using Hakeem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hakeem.Domain.Common;
using Microsoft.Extensions.Configuration;

namespace Hakeem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AuthController(IAuthService authService, IConfiguration configuration)
    {
        _authService = authService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);
        if (result.IsFailure)
        {
            return BadRequest(new { Error = result.Error.Message, Code = result.Error.Code });
        }
        return Ok(new { Message = "Registration successful. Please check your email to verify your account." });
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequestDto request)
    {
        var result = await _authService.VerifyEmailAsync(request);
        if (result.IsFailure)
        {
            return BadRequest(new { Error = result.Error.Message, Code = result.Error.Code });
        }
        return Ok(result.Value);
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification(ResendVerificationRequestDto request)
    {
        var result = await _authService.ResendVerificationAsync(request);
        if (result.IsFailure)
        {
            return BadRequest(new { Error = result.Error.Message, Code = result.Error.Code });
        }
        return Ok(new { Message = "Verification email sent if account exists and is not verified." });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new { Error = "Email is required.", Code = "Auth.EmailRequired" });

        var result = await _authService.ForgotPasswordAsync(email);
        if (result.IsFailure)
        {
            return BadRequest(new { Error = result.Error.Message, Code = result.Error.Code });
        }
        return Ok(new { Message = "If an account with this email exists, a password reset code has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        if (result.IsFailure)
        {
            return BadRequest(new { Error = result.Error.Message, Code = result.Error.Code });
        }
        return Ok(new { Message = "Password has been reset successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);
        if (result.IsFailure)
        {
            if (result.Error.Code == "Auth.EmailNotVerified")
            {
                return Unauthorized(new { Error = result.Error.Message, Code = result.Error.Code });
            }
            return BadRequest(new { Error = result.Error.Message, Code = result.Error.Code });
        }
        return Ok(result.Value);
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin(GoogleLoginRequestDto request)
    {
        var result = await _authService.GoogleLoginAsync(request);
        if (result.IsFailure)
        {
            return BadRequest(new { Error = result.Error.Message, Code = result.Error.Code });
        }
        return Ok(result.Value);
    }

    [HttpGet("google/login")]
    public IActionResult GoogleLoginRedirect()
    {
        var clientId = _configuration["Authentication:Google:ClientId"];
        var redirectUri = _configuration["Authentication:Google:CallbackUrl"];
        var googleAuthUrl = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope=openid%20email%20profile&access_type=offline";
        return Redirect(googleAuthUrl);
    }

    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest(new { Error = "No code provided by Google.", Code = "Auth.GoogleCodeMissing" });
        }

        var result = await _authService.GoogleCallbackAsync(code);
        if (result.IsFailure)
        {
            return BadRequest(new { Error = result.Error.Message, Code = result.Error.Code });
        }
        return Ok(result.Value);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        if (result.IsFailure)
        {
            return BadRequest(new { Error = result.Error.Message, Code = result.Error.Code });
        }
        return Ok(result.Value);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId != null)
        {
            await _authService.LogoutAsync(userId, request.RefreshToken);
        }
        return Ok();
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequestDto request)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var result = await _authService.ChangePasswordAsync(userId, request);
        if (result.IsFailure)
        {
            return BadRequest(new { Error = result.Error.Message, Code = result.Error.Code });
        }
        return Ok(new { Message = "Password has been changed successfully." });
    }

    [HttpPost("admin/create-user")]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateUserByAdmin()
    {
        return StatusCode(StatusCodes.Status501NotImplemented, "Admin user creation is not implemented yet.");
    }
}
