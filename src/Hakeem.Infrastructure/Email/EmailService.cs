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

    public async Task SendEmailVerificationAsync(string toEmail, string userName, string verificationCode, int expireSeconds, string culture = "ar")
    {
        int expireMinutes = expireSeconds / 60;
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
                            <p style='font-size: 16px; margin-bottom: 24px;'>شكراً لانضمامك إلى <strong>حكيم</strong>. نحن سعداء بوجودك معنا! لإكمال عملية التسجيل، يرجى استخدام رمز التحقق أدناه:</p>
                            <div style='text-align: center; margin: 32px 0;'>
                                <div style='background-color: #f0f4f8; border: 1px dashed #1A73E8; color: #1A73E8; padding: 20px 32px; border-radius: 6px; font-size: 32px; font-weight: bold; display: inline-block; letter-spacing: 4px;'>{verificationCode}</div>
                            </div>
                            <p style='font-size: 14px; color: #555555; text-align: center;'>هذا الرمز صالح لمدة <strong>{expireMinutes} دقائق</strong>.</p>
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
                            <p style='font-size: 16px; margin-bottom: 24px;'>Thank you for joining <strong>Hakeem</strong>. We are thrilled to have you! To complete your registration, please use the verification code below:</p>
                            <div style='text-align: center; margin: 32px 0;'>
                                <div style='background-color: #f0f4f8; border: 1px dashed #1A73E8; color: #1A73E8; padding: 20px 32px; border-radius: 6px; font-size: 32px; font-weight: bold; display: inline-block; letter-spacing: 4px;'>{verificationCode}</div>
                            </div>
                            <p style='font-size: 14px; color: #555555; text-align: center;'>This code is valid for <strong>{expireMinutes} minutes</strong>.</p>
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

    public async Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetCode, string culture = "ar")
    {
        var emailMessage = new MimeMessage();
        var fromAddress = _config["EmailSettings:FromAddress"] ?? "noreply@hakeem.app";
        emailMessage.From.Add(new MailboxAddress("Hakeem", fromAddress));
        emailMessage.To.Add(new MailboxAddress(userName, toEmail));
        
        var isArabic = culture.Equals("ar", StringComparison.OrdinalIgnoreCase);
        emailMessage.Subject = isArabic ? "إعادة تعيين كلمة المرور - حكيم" : "Reset your password - Hakeem";

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
                            <p style='font-size: 16px; margin-bottom: 24px;'>لقد تلقينا طلباً لإعادة تعيين كلمة المرور الخاصة بحسابك. استخدم الرمز أدناه لإتمام العملية:</p>
                            <div style='text-align: center; margin: 32px 0;'>
                                <div style='background-color: #f0f4f8; border: 1px dashed #1A73E8; color: #1A73E8; padding: 20px 32px; border-radius: 6px; font-size: 32px; font-weight: bold; display: inline-block; letter-spacing: 4px;'>{resetCode}</div>
                            </div>
                            <p style='font-size: 14px; color: #666666; margin-top: 32px; border-top: 1px solid #eeeeee; padding-top: 24px;'>إذا لم تطلب إعادة تعيين كلمة المرور، يمكنك تجاهل هذه الرسالة بأمان. لن يتم تغيير كلمة المرور الخاصة بك.</p>
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
                            <p style='font-size: 16px; margin-bottom: 24px;'>We received a request to reset your password. Use the code below to complete the process:</p>
                            <div style='text-align: center; margin: 32px 0;'>
                                <div style='background-color: #f0f4f8; border: 1px dashed #1A73E8; color: #1A73E8; padding: 20px 32px; border-radius: 6px; font-size: 32px; font-weight: bold; display: inline-block; letter-spacing: 4px;'>{resetCode}</div>
                            </div>
                            <p style='font-size: 14px; color: #666666; margin-top: 32px; border-top: 1px solid #eeeeee; padding-top: 24px;'>If you did not request a password reset, you can safely ignore this email. Your password will not be changed.</p>
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
