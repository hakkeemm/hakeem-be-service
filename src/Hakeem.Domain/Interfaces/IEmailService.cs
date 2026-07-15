namespace Hakeem.Domain.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationAsync(string toEmail, string userName, string verificationCode, int expireSeconds, string culture = "ar");
}
