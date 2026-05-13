using System.Linq.Expressions;
using Finanza.Domain.Entities;
using Finanza.Domain.Enums;

namespace Finanza.Domain.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> GetByIdAsync(Guid id);
        Task AddAsync(Transaction request);
        void Update(Transaction request);
        void Remove(Transaction request);
        Task<List<Transaction>> GetAllAsync();
        Task<List<Transaction>> GetByFilterAsync(Expression<Func<Transaction, bool>> predicate);
        Task<List<Transaction>> SearchAsync(string? search, TransactionStatus? status, TransactionType? type, DateTime? startDate, DateTime? endDate);
    }
}
