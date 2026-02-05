using Domain.Entities;

namespace Application.DTOs.Responses
{
    public class TransactionResponse
    {
        public Guid Id { get; private set; }
        public string Description { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime DueDate { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? PaymentDate { get; private set; }
        public string Status { get; private set; }
        public string Type { get; private set; }
        public string CategoryName { get; private set; }
        public bool IsOverdue { get; private set; }

        public static TransactionResponse Create(Transaction transaction)
        {
            return new TransactionResponse
            {
                Id = transaction.Id,
                Description = transaction.Description,
                Amount = transaction.Amount,
                DueDate = transaction.Dates.DueDate,
                CreatedAt = transaction.Dates.CreatedAt,
                PaymentDate = transaction.PaymentDate,
                Status = transaction.Status.ToString(),
                Type = transaction.Type.ToString(),
                CategoryName = transaction.Category?.Name ?? "Sem Categoria",
                IsOverdue = transaction.IsOverdue(DateTime.Today)
            };
        }
    }
}
