using FinanceManager.Domain.Abstractions;
using FinanceManager.Domain.Enums;
using FinanceManager.Domain.Events;
using FinanceManager.Domain.Exceptions;
using FinanceManager.Domain.ValueObjects;

namespace FinanceManager.Domain.Entities
{
    public class Transaction : Entity
    {
        protected Transaction() { }

        private Transaction(Guid id, Description description, Money amount, TransactionDates dates, TransactionType type, Guid categoryId)
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
                new Description(description),
                new Money(amount),
                new TransactionDates(dueDate, createdAt),
                type,
                categoryId
            );
        }

        public Guid Id { get; private set; }
        public Description Description { get; private set; }
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

            //if (paymentDate.Date < Dates.CreatedAt.Date)
            //    throw new TransactionPayException("A data de pagamento não pode ser anterior à data de criação da transação");

            PaymentDate = paymentDate;
            Status = TransactionStatus.Paid;

            AddDomainEvent(new TransactionPaidEvent(Id, paymentDate));
        }

        public void Reopen()
        {
            if (Status != TransactionStatus.Paid)
                throw new TransactionReopenException();

            Status = TransactionStatus.Pending;
            PaymentDate = null;

            AddDomainEvent(new TransactionReopenEvent(Id, Status));
        }

        public void Cancel()
        {
            if (Status == TransactionStatus.Cancelled)
                throw new TransactionCancelException("Não é possível cancelar uma transação que já foi cancelada");

            if (Status == TransactionStatus.Paid)
                throw new TransactionCancelException("Não é possível cancelar uma transação que já foi paga");

            Status = TransactionStatus.Cancelled;
            PaymentDate = null;

            AddDomainEvent(new TransactionCancelEvent(Id, Status));
        }

        public void Update(string description, decimal amount, DateTime dueDate, TransactionType type, Guid categoryId)
        {
            if (Status == TransactionStatus.Paid)
                throw new TransactionUpdateException("Não é possível editar uma transação que já foi paga");

            if (Status == TransactionStatus.Cancelled)
                throw new TransactionUpdateException("Não é possível editar uma transação que foi cancelada");

            Description = new Description(description);
            Amount = new Money(amount);
            Dates = new TransactionDates(dueDate, Dates.CreatedAt);
            Type = type;
            CategoryId = categoryId;
        }

        public bool IsOverdue(DateTime today) => Status == TransactionStatus.Pending && today.Date > Dates.DueDate.Date;
    }
}
