using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Net;
using System.Net.Mail;

namespace wapp.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Credentials = new NetworkCredential("olddonkeylearninglanguages@gmail.com", "gypl krrz pycm wrih"),
                Port = 587,
                EnableSsl = true,
            };


            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress("olddonkeylearninglanguages@gmail.com", "Aviso de sistema do trabalho de MVC"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            mailMessage.Bcc.Add("olddonkeylearninglanguages@gmail.com");

            smtpClient.Send(mailMessage);

            return Task.CompletedTask;
        }
    }
}