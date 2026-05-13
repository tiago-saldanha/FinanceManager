using Finanza.Domain.Interfaces;

namespace Finanza.Domain.Abstractions
{
    public abstract class DomainEvent : IDomainEvent
    {
        public DateTime OcurredAt { get; } = DateTime.UtcNow;
    }
}
