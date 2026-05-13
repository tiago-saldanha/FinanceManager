using Finanza.Domain.Exceptions;

namespace Finanza.Domain.ValueObjects
{
    public readonly record struct TransactionDates
    {
        public DateTime DueDate { get; }
        public DateTime CreatedAt { get; }
        
        public TransactionDates(DateTime dueDate, DateTime createdAt)
        {
            DueDate = dueDate;
            CreatedAt = createdAt;
        }
    }
}
