using Finanza.Domain.Abstractions;
using Finanza.Domain.Enums;

namespace Finanza.Domain.Events
{
    public class TransactionCancelEvent(Guid transactionId, TransactionStatus status) : DomainEvent
    {
        public Guid TransactionId { get; } = transactionId;
        public TransactionStatus Status { get; } = status;
    }
}
