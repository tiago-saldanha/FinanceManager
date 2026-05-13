using Finanza.Domain.Abstractions;
using Finanza.Domain.Interfaces;

namespace Finanza.Domain.Events
{
    public class TransactionPaidEvent(Guid transactionId, DateTime paymentDate) : DomainEvent
    {
        public Guid TransactionId { get; } = transactionId;
        public DateTime PaymentDate { get; } = paymentDate;
    }
}
