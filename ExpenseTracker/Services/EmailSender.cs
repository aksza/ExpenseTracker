using ExpenseTracker.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace ExpenseTracker.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _settings;

        public EmailSender(IOptions<SmtpSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var client = new SmtpClient(_settings.Server, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);
        }
    }
}
