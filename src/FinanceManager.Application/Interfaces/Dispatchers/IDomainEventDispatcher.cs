using Domain.Interfaces;

namespace Application.Interfaces.Dispatchers
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IEnumerable<IDomainEvent> events);
    }
}
