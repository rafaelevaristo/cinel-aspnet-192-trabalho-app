using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net;
using System.Net.Mail;
using waap.ServiceModels;

namespace wapp.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (SmtpClient smtpClient = new SmtpClient(_emailSettings.SmtpServer))
            {
                smtpClient.Credentials = new NetworkCredential(_emailSettings.Email, _emailSettings.Password);
                smtpClient.Port = _emailSettings.Port;
                smtpClient.EnableSsl = _emailSettings.EnableSsl;

                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.Email, _emailSettings.FriendlyName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);
                mailMessage.Bcc.Add(_emailSettings.Email);

                if (_emailSettings.SendNotificationEmail)
                {
                    smtpClient.Send(mailMessage);
                }

                
            }

            return Task.CompletedTask;
        }
    }
}