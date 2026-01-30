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
                throw new TransactionException("A data de vencimento não pode ser anterior à data de criação");

            DueDate = dueDate;
            CreatedAt = createdAt;
        }
    }
}
