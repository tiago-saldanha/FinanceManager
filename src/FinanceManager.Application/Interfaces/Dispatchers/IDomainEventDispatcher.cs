using FinanceManager.Domain.Interfaces;

namespace FinanceManager.Application.Interfaces.Dispatchers
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IEnumerable<IDomainEvent> events);
    }
}
