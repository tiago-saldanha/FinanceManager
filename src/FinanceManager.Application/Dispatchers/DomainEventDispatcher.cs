using Application.Interfaces.Dispatchers;
using Application.Interfaces.Handlers;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Dispatchers
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
