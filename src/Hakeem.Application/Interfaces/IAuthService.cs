using Hakeem.Application.DTOs.Auth;

namespace Hakeem.Application.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> VerifyEmailAsync(VerifyEmailRequestDto request);
    Task ResendVerificationAsync(ResendVerificationRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> GoogleLoginAsync(GoogleLoginRequestDto request);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task LogoutAsync(string userId, string refreshToken);
}
