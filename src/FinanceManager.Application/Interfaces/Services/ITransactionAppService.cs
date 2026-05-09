using FinanceManager.Application.DTOs.Requests;
using FinanceManager.Application.DTOs.Responses;
using FinanceManager.Application.Enums;

namespace FinanceManager.Application.Interfaces.Services
{
    public interface ITransactionAppService
    {
        Task<IEnumerable<TransactionResponse>> GetAllAsync();
        Task<IEnumerable<TransactionResponse>> GetByStatusAsync(TransactionStatusDto status);
        Task<IEnumerable<TransactionResponse>> GetByTypeAsync(TransactionTypeDto type);
        Task<IEnumerable<TransactionResponse>> SearchAsync(string? search, TransactionStatusDto? status, TransactionTypeDto? type, DateTime? startDate, DateTime? endDate);
        Task<TransactionResponse> CreateAsync(CreateTransactionRequest request);
        Task<TransactionResponse> UpdateAsync(Guid id, UpdateTransactionRequest request);
        Task<TransactionResponse> PayAsync(Guid id, PayTransactionRequest request);
        Task<TransactionResponse> ReopenAsync(Guid id);
        Task<TransactionResponse> CancelAsync(Guid id);
        Task<TransactionResponse> GetByIdAsync(Guid id);
        Task<TransactionResponse> RemoveByIdAsync(Guid id);
    }
}
