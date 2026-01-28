using API.Domain.Enums;

namespace API.Domain.Entities
{
    public class Transaction
    {
        protected Transaction() { }

        public Transaction(Guid id, string description, decimal amount, DateTime dueDate, TransactionType type, Guid categoryId, DateTime createdAt)
        {
            Id = id;
            Description = description;
            Amount = amount;
            DueDate = dueDate;
            Type = type;
            CategoryId = categoryId;
            Status = TransactionStatus.Pending;
            CreatedAt = createdAt;
        }

        public static Transaction Create(string description, decimal amount, DateTime dueDate, string type, Guid categoryId)
            => new(Guid.NewGuid(), description, amount, dueDate, Enum.Parse<TransactionType>(type, true), categoryId, DateTime.UtcNow);

        public Guid Id { get; private set; }
        public string Description { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime DueDate { get; private set; }
        public DateTime? PaymentDate { get; private set; }
        public TransactionStatus Status { get; private set; }
        public TransactionType Type { get; private set; }
        public Guid CategoryId { get; private set; }
        public virtual Category Category { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public void Pay(DateTime paymentDate)
        {
            if (Status == TransactionStatus.Cancelled)
                throw new InvalidOperationException("Não é possível pagar uma transação cancelada.");

            if (Status == TransactionStatus.Paid)
                throw new InvalidOperationException("Esta transação já foi paga.");

            if (paymentDate.Date < CreatedAt.Date)
                throw new InvalidOperationException("A data de pagamento não pode ser anterior à data de criação da conta.");

            PaymentDate = paymentDate;
            Status = TransactionStatus.Paid;
        }

        public void Cancel()
        {
            if (Status == TransactionStatus.Paid)
                throw new InvalidOperationException("Não é possível cancelar uma transação já paga.");

            Status = TransactionStatus.Cancelled;
            PaymentDate = null;
        }

        public bool IsOverdue => Status == TransactionStatus.Pending && DateTime.Today > DueDate.Date;
    }
}
