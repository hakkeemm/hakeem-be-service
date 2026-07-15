using Hakeem.Domain.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Hakeem.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailVerificationAsync(string toEmail, string userName, string verificationLink, string culture = "ar")
    {
        var emailMessage = new MimeMessage();
        var fromAddress = _config["EmailSettings:FromAddress"] ?? "noreply@hakeem.app";
        emailMessage.From.Add(new MailboxAddress("Hakeem", fromAddress));
        emailMessage.To.Add(new MailboxAddress(userName, toEmail));
        
        var isArabic = culture.Equals("ar", StringComparison.OrdinalIgnoreCase);
        emailMessage.Subject = isArabic ? "تأكيد بريدك الإلكتروني - حكيم" : "Confirm your email - Hakeem";

        var bodyBuilder = new BodyBuilder();
        if (isArabic)
        {
            bodyBuilder.HtmlBody = $@"
                <div dir='rtl' style='font-family: Arial, sans-serif;'>
                    <h2>أهلاً {{userName}}،</h2>
                    <p>شكراً لتسجيلك في حكيم. يرجى تأكيد بريدك الإلكتروني بالضغط على الرابط أدناه:</p>
                    <p><a href='{{verificationLink}}'>تأكيد البريد الإلكتروني</a></p>
                    <p>إذا لم تقم بإنشاء حساب، يمكنك تجاهل هذه الرسالة.</p>
                </div>
            ";
        }
        else
        {
            bodyBuilder.HtmlBody = $@"
                <div style='font-family: Arial, sans-serif;'>
                    <h2>Hello {{userName}},</h2>
                    <p>Thank you for registering with Hakeem. Please confirm your email by clicking the link below:</p>
                    <p><a href='{{verificationLink}}'>Confirm Email</a></p>
                    <p>If you did not create an account, you can ignore this email.</p>
                </div>
            ";
        }

        emailMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        
        var host = _config["EmailSettings:Host"] ?? "localhost";
        var port = int.Parse(_config["EmailSettings:Port"] ?? "587");
        var username = _config["EmailSettings:Username"];
        var password = _config["EmailSettings:Password"] ?? string.Empty;

        try
        {
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            if (!string.IsNullOrEmpty(username))
            {
                await client.AuthenticateAsync(username, password);
            }
            await client.SendAsync(emailMessage);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
