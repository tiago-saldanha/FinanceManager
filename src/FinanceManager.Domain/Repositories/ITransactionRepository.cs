using System.Linq.Expressions;
using Domain.Entities;

namespace Domain.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> GetByIdAsync(Guid id);
        Task AddAsync(Transaction request);
        void Update(Transaction request);
        Task<List<Transaction>> GetAllAsync();
        Task<List<Transaction>> GetByFilterAsync(Expression<Func<Transaction, bool>> predicate);
    }
}
