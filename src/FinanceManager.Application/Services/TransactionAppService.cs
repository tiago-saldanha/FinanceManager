using FinanceManager.Domain.Repositories;
using FinanceManager.Application.DTOs.Requests;
using FinanceManager.Application.DTOs.Responses;
using FinanceManager.Application.Enums;
using FinanceManager.Application.Interfaces.Services;
using FinanceManager.Application.Interfaces.Dispatchers;

namespace FinanceManager.Application.Services
{
    public class TransactionAppService(ITransactionRepository repository, IUnitOfWork unitOfWork, IDomainEventDispatcher dispatcher) : ITransactionAppService
    {
        public async Task<TransactionResponse> GetByIdAsync(Guid id)
            => TransactionResponse.Create(await repository.GetByIdAsync(id));

        public async Task<IEnumerable<TransactionResponse>> GetAllAsync()
        {
            var transactions = await repository.GetAllAsync();
            return transactions.Select(TransactionResponse.Create);
        }

        public async Task<IEnumerable<TransactionResponse>> GetByStatusAsync(TransactionStatusDto status)
        {
            var transactions = await repository.GetByFilterAsync(q => q.Status == Mapper.Mapper.TransactionStatus(status));
            return transactions.Select(TransactionResponse.Create);
        }

        public async Task<IEnumerable<TransactionResponse>> GetByTypeAsync(TransactionTypeDto type)
        {
            var transactions = await repository.GetByFilterAsync(q => q.Type == Mapper.Mapper.TransactionType(type));
            return transactions.Select(TransactionResponse.Create);
        }

        public async Task<IEnumerable<TransactionResponse>> SearchAsync(string? search, TransactionStatusDto? status, TransactionTypeDto? type, DateTime? startDate, DateTime? endDate)
        {
            var domainStatus = status.HasValue ? Mapper.Mapper.TransactionStatus(status.Value) : (Domain.Enums.TransactionStatus?)null;
            var domainType = type.HasValue ? Mapper.Mapper.TransactionType(type.Value) : (Domain.Enums.TransactionType?)null;
            var transactions = await repository.SearchAsync(search, domainStatus, domainType, startDate, endDate);
            return transactions.Select(TransactionResponse.Create);
        }

        public async Task<TransactionResponse> PayAsync(Guid id, PayTransactionRequest request)
        {
            var transaction = await repository.GetByIdAsync(id);
            transaction.Pay(request.PaymentDate);
            repository.Update(transaction);
            await unitOfWork.CommitAsync();
            await dispatcher.DispatchAsync(transaction.DomainEvents);
            return TransactionResponse.Create(transaction);
        }

        public async Task<TransactionResponse> ReopenAsync(Guid id)
        {
            var transaction = await repository.GetByIdAsync(id);
            transaction.Reopen();
            repository.Update(transaction);
            await unitOfWork.CommitAsync();
            await dispatcher.DispatchAsync(transaction.DomainEvents);
            return TransactionResponse.Create(transaction);
        }

        public async Task<TransactionResponse> CancelAsync(Guid id)
        {
            var transaction = await repository.GetByIdAsync(id);
            transaction.Cancel();
            repository.Update(transaction);
            await unitOfWork.CommitAsync();
            await dispatcher.DispatchAsync(transaction.DomainEvents);
            return TransactionResponse.Create(transaction);
        }

        public async Task<TransactionResponse> CreateAsync(CreateTransactionRequest request)
        {
            var transaction = request.ToEntity();
            await repository.AddAsync(transaction);
            await unitOfWork.CommitAsync();
            return TransactionResponse.Create(transaction);
        }

        public async Task<TransactionResponse> UpdateAsync(Guid id, UpdateTransactionRequest request)
        {
            var transaction = await repository.GetByIdAsync(id);
            transaction.Update(
                request.Description,
                request.Amount,
                request.DueDate,
                Mapper.Mapper.TransactionType(request.TransactionType),
                request.CategoryId
            );
            repository.Update(transaction);
            await unitOfWork.CommitAsync();
            return TransactionResponse.Create(transaction);
        }

        public async Task<TransactionResponse> RemoveByIdAsync(Guid id)
        {
            var transaction = await repository.GetByIdAsync(id);
            repository.Remove(transaction);
            await unitOfWork.CommitAsync();
            return TransactionResponse.Create(transaction);
        }
    }
}
