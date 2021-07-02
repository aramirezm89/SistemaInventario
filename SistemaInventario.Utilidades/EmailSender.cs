
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SistemaInventario.Utilidades
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(subject, htmlMessage, email);
        }

        public Task Execute(string subject, string message, string email)
        {
            MailMessage newMessage = new MailMessage();
            newMessage.To.Add(email);
            newMessage.Subject = subject;
            newMessage.Body = message;
            newMessage.From = new MailAddress("aramirez.unimarc@gmail.com");
            newMessage.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.sendgrid.net");
            smtp.Port = 465;
            smtp.UseDefaultCredentials = true;
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential("apikey", "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");

            return smtp.SendMailAsync(newMessage);

        }
    }
}
