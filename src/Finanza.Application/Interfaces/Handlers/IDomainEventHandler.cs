using Finanza.Domain.Interfaces;

namespace Finanza.Application.Interfaces.Handlers
{
    public interface IDomainEventHandler<in TEvent>
        where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent @event);
    }
}
