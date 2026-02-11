using Application.Interfaces.Handlers;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.Handlers
{
    public class TransactionCancelEventHandler(ILogger<TransactionCancelEventHandler> logger)
        : IDomainEventHandler<TransactionCancelEvent>
    {
        public async Task HandleAsync(TransactionCancelEvent @event)
        {
            logger.LogInformation("[TransactionCancelEvent] - Transaction Id: {TransactionId}, Status: {Status}, Ocurred At UTC: {OcurredAt}", @event.TransactionId, @event.Status, @event.OcurredAt);
            await Task.Delay(500);
        }
    }
}
