namespace FinanceManager.Application.DTOs.Requests
{
    public class UpdateTransactionRequest
    {
        public string Description { get; set; } = default!;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string TransactionType { get; set; } = default!;
        public Guid CategoryId { get; set; }
    }
}
