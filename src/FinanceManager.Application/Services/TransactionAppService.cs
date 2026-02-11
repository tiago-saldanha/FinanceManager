using Domain.Repositories;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Enums;
using Application.Interfaces.Services;
using Application.Interfaces.Dispatchers;

namespace Application.Services
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
    }
}
