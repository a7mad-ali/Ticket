using System.Threading.Tasks;

namespace Ticket.Domain.Contracts.Interfaces.IService
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
