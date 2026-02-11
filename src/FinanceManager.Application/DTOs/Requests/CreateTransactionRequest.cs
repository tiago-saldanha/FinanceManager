using Domain.Entities;
using Domain.Enums;

namespace Application.DTOs.Requests
{
    public class CreateTransactionRequest
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string TransactionType { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Transaction ToEntity()
        {
            return Transaction.Create(
                Description,
                Amount,
                DueDate,
                Map(TransactionType),
                CategoryId,
                CreatedAt
            );
        }

        private TransactionType Map(string transactionType) => Mapper.Mapper.TransactionType(transactionType);
    }
}
