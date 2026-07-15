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
                <div dir='rtl' style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7f6; padding: 40px 20px; color: #333333; line-height: 1.6;'>
                    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.05);'>
                        <div style='background-color: #1A73E8; padding: 24px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: 600;'>حكيم</h1>
                        </div>
                        <div style='padding: 40px 32px;'>
                            <h2 style='margin-top: 0; font-size: 24px; color: #1A73E8;'>أهلاً {userName}،</h2>
                            <p style='font-size: 16px; margin-bottom: 24px;'>شكراً لانضمامك إلى <strong>حكيم</strong>. نحن سعداء بوجودك معنا! لإكمال عملية التسجيل، يرجى تأكيد بريدك الإلكتروني بالضغط على الزر أدناه.</p>
                            <div style='text-align: center; margin: 32px 0;'>
                                <a href='{verificationLink}' style='background-color: #1A73E8; color: #ffffff; text-decoration: none; padding: 14px 32px; border-radius: 6px; font-size: 16px; font-weight: bold; display: inline-block;'>تأكيد البريد الإلكتروني</a>
                            </div>
                            <p style='font-size: 14px; color: #666666; margin-top: 32px; border-top: 1px solid #eeeeee; padding-top: 24px;'>إذا لم تقم بإنشاء حساب في حكيم، يرجى تجاهل هذه الرسالة بأمان.</p>
                        </div>
                    </div>
                </div>
            ";
        }
        else
        {
            bodyBuilder.HtmlBody = $@"
                <div dir='ltr' style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7f6; padding: 40px 20px; color: #333333; line-height: 1.6;'>
                    <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 6px rgba(0,0,0,0.05);'>
                        <div style='background-color: #1A73E8; padding: 24px; text-align: center;'>
                            <h1 style='color: #ffffff; margin: 0; font-size: 28px; font-weight: 600;'>Hakeem</h1>
                        </div>
                        <div style='padding: 40px 32px;'>
                            <h2 style='margin-top: 0; font-size: 24px; color: #1A73E8;'>Hello {userName},</h2>
                            <p style='font-size: 16px; margin-bottom: 24px;'>Thank you for joining <strong>Hakeem</strong>. We are thrilled to have you! To complete your registration, please confirm your email address by clicking the button below.</p>
                            <div style='text-align: center; margin: 32px 0;'>
                                <a href='{verificationLink}' style='background-color: #1A73E8; color: #ffffff; text-decoration: none; padding: 14px 32px; border-radius: 6px; font-size: 16px; font-weight: bold; display: inline-block;'>Confirm Email</a>
                            </div>
                            <p style='font-size: 14px; color: #666666; margin-top: 32px; border-top: 1px solid #eeeeee; padding-top: 24px;'>If you did not create a Hakeem account, please safely ignore this email.</p>
                        </div>
                    </div>
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
