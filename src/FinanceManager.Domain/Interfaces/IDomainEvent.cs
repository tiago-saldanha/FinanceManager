namespace Domain.Interfaces
{
    public interface IDomainEvent
    {
        DateTime OcurredAt { get; }
    }
}
