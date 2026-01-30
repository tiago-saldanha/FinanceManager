using API.Domain.Exceptions;

namespace API.Domain.ValueObjects
{
    public sealed record class TransactionDates
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
