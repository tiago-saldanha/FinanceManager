using Domain.Exceptions;

namespace Domain.ValueObjects
{
    public readonly record struct TransactionDates
    {
        public DateTime DueDate { get; }
        public DateTime CreatedAt { get; }
        
        public TransactionDates(DateTime dueDate, DateTime createdAt)
        {
            if (dueDate.Date < createdAt.Date)
                throw new TransactionDateException();

            DueDate = dueDate;
            CreatedAt = createdAt;
        }
    }
}
