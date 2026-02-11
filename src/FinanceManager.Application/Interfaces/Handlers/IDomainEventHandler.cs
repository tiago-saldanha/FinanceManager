using FinanceManager.Domain.Interfaces;

namespace FinanceManager.Application.Interfaces.Handlers
{
    public interface IDomainEventHandler<in TEvent>
        where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent @event);
    }
}
