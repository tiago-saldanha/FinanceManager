using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Events
{
    public class TransactionReopenEvent(Guid transactionId, TransactionStatus status) : DomainEvent
    {
        public Guid TransactionId { get; } = transactionId;
        public TransactionStatus Status { get; } = status;
    }
}
