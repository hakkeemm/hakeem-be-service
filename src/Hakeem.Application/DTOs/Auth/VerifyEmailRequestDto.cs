namespace Hakeem.Application.DTOs.Auth;

public class VerifyEmailRequestDto
{
    public string UserId { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
