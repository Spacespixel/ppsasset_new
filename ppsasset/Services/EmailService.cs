using MailKit.Net.Smtp;
using MailKit;
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
                var host = emailSettings["SmtpServer"] ?? emailSettings["MailServer"];
                var portString = emailSettings["SmtpPort"] ?? emailSettings["MailPort"] ?? "25";
                var port = int.Parse(portString);
                var senderName = emailSettings["SenderName"];
                var senderEmail = emailSettings["SenderEmail"];
                var smtpUsername = emailSettings["SmtpUsername"];
                var senderPassword = emailSettings["SmtpPassword"] ?? emailSettings["SenderPassword"];
                var enableSslStr = emailSettings["EnableSsl"];
                var enableSsl = !string.IsNullOrEmpty(enableSslStr) ? bool.Parse(enableSslStr) : (port == 587 || port == 465);


                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(senderEmail))
                {
                    _logger.LogWarning("Email settings are incomplete. Host: {Host}, Email: {Email}", host, senderEmail);
                    return;
                }

                _logger.LogInformation("Sending email via {Host}:{Port} as {SenderEmail}", host, port, senderEmail);

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(senderName, senderEmail));
                email.To.AddRange(InternetAddressList.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(isHtml ? TextFormat.Html : TextFormat.Plain) { Text = body };

                using var smtp = new SmtpClient(new ProtocolLogger(Console.OpenStandardOutput()));
                
                // If checking certificate revocation fails (common in dev), user might want to ignore verify.
                smtp.CheckCertificateRevocation = false;

                var socketOptions = enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
                await smtp.ConnectAsync(host, port, socketOptions);
                
                if (!string.IsNullOrEmpty(senderPassword))
                {
                    var username = !string.IsNullOrEmpty(smtpUsername) ? smtpUsername : senderEmail;
                    await smtp.AuthenticateAsync(username, senderPassword);
                }

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Recipient}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}. Error: {Message}", to, ex.Message);
                // We don't throw here to avoid breaking the registration flow if email fails
            }
        }
    }
}
