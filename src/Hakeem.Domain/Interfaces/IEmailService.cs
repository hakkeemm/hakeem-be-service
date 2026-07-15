namespace Hakeem.Domain.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationAsync(string toEmail, string userName, string verificationLink, string culture = "ar");
}
