using API.Domain.Enums;
using API.Domain.Exceptions;

namespace API.Domain.Entities
{
    public class Transaction
    {
        protected Transaction() { }

        private Transaction(Guid id, string description, decimal amount, DateTime dueDate, TransactionType type, Guid categoryId, DateTime createdAt)
        {
            if (string.IsNullOrEmpty(description)) 
                throw new TransactionException("A descrição da transação deve ser informada");
            if (amount <= 0) 
                throw new TransactionException("O valor da transação deve ser maior que 0");
            if (dueDate.Date < createdAt.Date) 
                throw new TransactionException("A data de vencimento não pode ser anterior à data de criação");

            Id = id;
            Description = description;
            Amount = amount;
            DueDate = dueDate;
            Type = type;
            CategoryId = categoryId;
            Status = TransactionStatus.Pending;
            CreatedAt = createdAt;
        }

        public static Transaction Create(string description, decimal amount, DateTime dueDate, TransactionType type, Guid categoryId, DateTime CreatedAt)
            => new(Guid.NewGuid(), description, amount, dueDate, type, categoryId, CreatedAt);

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
                throw new TransactionException("A transação já foi cancelada");

            if (Status == TransactionStatus.Paid)
                throw new TransactionException("A transação já foi paga");

            if (paymentDate.Date < CreatedAt.Date)
                throw new TransactionException("A data de pagamento não pode ser anterior à data de criação da transação");

            PaymentDate = paymentDate;
            Status = TransactionStatus.Paid;
        }

        public void Unpay()
        {
            if (Status != TransactionStatus.Paid)
                throw new TransactionException("A transação não está paga");

            Status = TransactionStatus.Pending;
            PaymentDate = null;
        }

        public void Cancel()
        {
            if (Status == TransactionStatus.Paid)
                throw new TransactionException("Não é possível cancelar uma transação que já foi paga");

            Status = TransactionStatus.Cancelled;
            PaymentDate = null;
        }

        public bool IsOverdue => Status == TransactionStatus.Pending && DateTime.Today > DueDate.Date;
    }
}
