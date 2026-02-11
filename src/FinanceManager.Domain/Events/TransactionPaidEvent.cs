using Domain.Abstractions;
using Domain.Interfaces;

namespace Domain.Events
{
    public class TransactionPaidEvent(Guid transactionId, DateTime paymentDate) : DomainEvent
    {
        public Guid TransactionId { get; } = transactionId;
        public DateTime PaymentDate { get; } = paymentDate;
    }
}
