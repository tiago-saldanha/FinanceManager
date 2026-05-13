using Finanza.Domain.Interfaces;

namespace Finanza.Application.Interfaces.Dispatchers
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IEnumerable<IDomainEvent> events);
    }
}
