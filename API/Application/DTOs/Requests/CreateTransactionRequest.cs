namespace API.Application.DTOs.Requests
{
    public record CreateTransactionRequest(
        string Description,
        decimal Amount,
        DateTime DueDate,
        string Type,
        Guid CategoryId
    );
}
