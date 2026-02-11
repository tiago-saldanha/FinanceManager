using Application.Interfaces.Handlers;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.Handlers
{
    public class TransactionPaidEventHandler(ILogger<TransactionPaidEventHandler> logger)
        : IDomainEventHandler<TransactionPaidEvent>
    {
        public async Task HandleAsync(TransactionPaidEvent @event)
        {
            logger.LogInformation("[TransactionPaidEvent] - Transaction Id: {TransactionId}, Payment Date: {PaymentDate}, Ocurred At UTC: {OcurredAt}", @event.TransactionId, @event.PaymentDate, @event.OcurredAt);
            await Task.Delay(500);
        }
    }
}
