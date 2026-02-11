using Application.Interfaces.Handlers;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.Handlers
{
    public class TransactionReopenEventHandler(ILogger<TransactionReopenEventHandler> logger)
        : IDomainEventHandler<TransactionReopenEvent>
    {
        public async Task HandleAsync(TransactionReopenEvent @event)
        {
            logger.LogInformation("[TransactionReopenEvent] - Transaction Id: {TransactionId}, Status: {Status}, Ocurred At UTC: {OcurredAt}", @event.TransactionId, @event.Status, @event.OcurredAt);
            await Task.Delay(500);
        }
    }
}
