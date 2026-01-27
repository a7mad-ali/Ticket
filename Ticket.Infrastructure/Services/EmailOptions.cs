namespace Ticket.Infrastructure.Services
{
    public class EmailOptions
    {
        public const string SectionName = "Email";

        public string Host { get; set; } 
        public int Port { get; set; } 
       
        public string SenderEmail { get; set; } 
        public string SenderPassword { get; set; } 
        public string AdminEmail { get; set; }
        public string VerificationBaseUrl { get; set; } = string.Empty;
    }
}
