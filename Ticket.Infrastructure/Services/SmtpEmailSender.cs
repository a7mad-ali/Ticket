using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Ticket.Domain.Contracts.Interfaces.IService;

namespace Ticket.Infrastructure.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailOptions _options;

        public SmtpEmailSender(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            using var message = new MailMessage
            {
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
                From = new MailAddress(_options.SenderEmail, _options.AdminEmail)
            };

            message.To.Add(new MailAddress(toEmail));

            using var client = new SmtpClient(_options.Host, _options.Port)
            {
               
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_options.AdminEmail, _options.SenderPassword)
            };


            await client.SendMailAsync(message);
        }
    }
}
