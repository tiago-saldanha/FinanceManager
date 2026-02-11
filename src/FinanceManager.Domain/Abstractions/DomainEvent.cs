using Domain.Interfaces;

namespace Domain.Abstractions
{
    public abstract class DomainEvent : IDomainEvent
    {
        public DateTime OcurredAt { get; } = DateTime.UtcNow;
    }
}
