using Hakeem.Application.DTOs.Auth;
using Hakeem.Domain.Common;

namespace Hakeem.Application.Interfaces;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterRequestDto request);
    Task<Result<AuthResponseDto>> VerifyEmailAsync(VerifyEmailRequestDto request);
    Task<Result> ResendVerificationAsync(ResendVerificationRequestDto request);
    Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    Task<Result<AuthResponseDto>> GoogleLoginAsync(GoogleLoginRequestDto request);
    Task<Result<AuthResponseDto>> GoogleCallbackAsync(string code);
    Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<Result> LogoutAsync(string userId, string refreshToken);
}
