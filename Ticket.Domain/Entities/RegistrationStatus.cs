namespace Ticket.Domain.Entities
{
    public enum RegistrationStatus
    {
        NotRegistered = 0,
        PendingVerification = 1,
        Verified = 2,
        Locked = 3
    }
}
