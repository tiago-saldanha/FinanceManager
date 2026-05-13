using Finanza.Domain.Interfaces;

namespace Finanza.Domain.Abstractions
{
    public abstract class Entity
    {
        private readonly List<IDomainEvent> _domainEvents = [];
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

        protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        protected void ClearDomainEvents() => _domainEvents.Clear();
    }
}
