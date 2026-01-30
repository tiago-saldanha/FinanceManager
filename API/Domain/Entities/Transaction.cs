using API.Domain.Enums;
using API.Domain.Exceptions;
using API.Domain.ValueObjects;

namespace API.Domain.Entities
{
    public class Transaction
    {
        protected Transaction() { }

        private Transaction(Guid id, TransactionDescription description, Money amount, TransactionDates dates, TransactionType type, Guid categoryId)
        {
            Id = id;
            Description = description;
            Amount = amount;
            Dates = dates;
            Type = type;
            CategoryId = categoryId;
            Status = TransactionStatus.Pending;
        }

        public static Transaction Create(string description, decimal amount, DateTime dueDate, TransactionType type, Guid categoryId, DateTime createdAt)
        {
            return new Transaction(
                Guid.NewGuid(),
                new TransactionDescription(description),
                new Money(amount),
                new TransactionDates(dueDate, createdAt),
                type,
                categoryId
            );
        }

        public Guid Id { get; private set; }
        public TransactionDescription Description { get; private set; }
        public Money Amount { get; private set; }
        public TransactionDates Dates { get; set; }
        public DateTime? PaymentDate { get; private set; }
        public TransactionStatus Status { get; private set; }
        public TransactionType Type { get; private set; }
        public Guid CategoryId { get; private set; }
        public virtual Category Category { get; private set; }

        public void Pay(DateTime paymentDate)
        {
            if (Status == TransactionStatus.Cancelled)
                throw new TransactionPayException("A transação já foi cancelada");

            if (Status == TransactionStatus.Paid)
                throw new TransactionPayException("A transação já foi paga");

            if (paymentDate.Date < Dates.CreatedAt.Date)
                throw new TransactionPayException("A data de pagamento não pode ser anterior à data de criação da transação");

            PaymentDate = paymentDate;
            Status = TransactionStatus.Paid;
        }

        public void Reopen()
        {
            if (Status != TransactionStatus.Paid)
                throw new TransactionReopenException();

            Status = TransactionStatus.Pending;
            PaymentDate = null;
        }

        public void Cancel()
        {
            if (Status == TransactionStatus.Paid)
                throw new TransactionCancelException();

            Status = TransactionStatus.Cancelled;
            PaymentDate = null;
        }

        public bool IsOverdue => Status == TransactionStatus.Pending && DateTime.Today > Dates.DueDate.Date;
    }
}
