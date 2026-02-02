namespace Ticket.Domain.Entities
{
    public enum RegistrationStatus
    {
        NotRegistered = 0,
        PendingEmailVerification = 1,
        Verified = 2
    }
}
