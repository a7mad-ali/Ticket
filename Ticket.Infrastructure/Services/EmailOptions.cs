namespace Ticket.Infrastructure.Services
{
    public class EmailOptions
    {
        public const string SectionName = "Email";

        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 25;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromAddress { get; set; } = string.Empty;
        public string FromName { get; set; } = "Ticket Support";
        public bool EnableSsl { get; set; } = true;
        public string VerificationBaseUrl { get; set; } = string.Empty;
    }
}
