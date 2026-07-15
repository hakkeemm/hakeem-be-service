using Hakeem.Application.DTOs.Auth;
using Hakeem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hakeem.Domain.Common;

namespace Hakeem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
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

    [HttpPost("admin/create-user")]
    [Authorize(Roles = "Admin")]
    public IActionResult CreateUserByAdmin()
    {
        return StatusCode(StatusCodes.Status501NotImplemented, "Admin user creation is not implemented yet.");
    }
}
