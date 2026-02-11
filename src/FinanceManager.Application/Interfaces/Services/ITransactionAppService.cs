using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Enums;

namespace Application.Interfaces.Services
{
    public interface ITransactionAppService
    {
        Task<IEnumerable<TransactionResponse>> GetAllAsync();
        Task<IEnumerable<TransactionResponse>> GetByStatusAsync(TransactionStatusDto status);
        Task<IEnumerable<TransactionResponse>> GetByTypeAsync(TransactionTypeDto type);
        Task<TransactionResponse> CreateAsync(CreateTransactionRequest request);
        Task<TransactionResponse> PayAsync(Guid id, PayTransactionRequest request);
        Task<TransactionResponse> ReopenAsync(Guid id);
        Task<TransactionResponse> CancelAsync(Guid id);
        Task<TransactionResponse> GetByIdAsync(Guid id);
    }
}
