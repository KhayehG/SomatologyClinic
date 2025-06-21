using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SomatologyClinic.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings, ILogger<EmailSender> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                _logger.LogInformation($"Attempting to send email to {email} with subject: {subject}");

                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SmtpUsername, _emailSettings.SenderName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                _logger.LogInformation("Sending email...");
                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {email}: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SenderName { get; set; }
    }
}