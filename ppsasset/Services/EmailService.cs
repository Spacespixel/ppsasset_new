using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PPSAsset.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await SendEmailAsync(to, subject, body, false);
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var host = emailSettings["MailServer"];
                var port = int.Parse(emailSettings["MailPort"] ?? "587");
                var senderName = emailSettings["SenderName"];
                var senderEmail = emailSettings["SenderEmail"];
                var senderPassword = emailSettings["SenderPassword"];

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(senderPassword))
                {
                    _logger.LogWarning("Email settings are incomplete. Skipping email sending.");
                    return;
                }

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(senderName, senderEmail));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(isHtml ? TextFormat.Html : TextFormat.Plain) { Text = body };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(senderEmail, senderPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Recipient}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", to);
                // We don't throw here to avoid breaking the registration flow if email fails
            }
        }
    }
}
