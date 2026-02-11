using FinanceManager.Application.Interfaces.Dispatchers;
using FinanceManager.Application.Interfaces.Handlers;
using FinanceManager.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManager.Application.Dispatchers
{
    public class DomainEventDispatcher(IServiceProvider provider)
        : IDomainEventDispatcher
    {
        public async Task DispatchAsync(IEnumerable<IDomainEvent> events)
        {
            foreach (var @event in events)
            {
                var type = typeof(IDomainEventHandler<>).MakeGenericType(@event.GetType());

                var handlers = provider.GetServices(type);

                foreach (var handler in handlers)
                {
                    if (handler != null)
                        await ((dynamic)handler).HandleAsync((dynamic)@event);
                }
            }
        }
    }
}
