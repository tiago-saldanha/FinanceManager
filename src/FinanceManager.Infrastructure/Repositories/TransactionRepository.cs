using System.Linq.Expressions;
using FinanceManager.Domain.Entities;
using FinanceManager.Domain.Repositories;
using FinanceManager.Infrastructure.Exceptions;
using FinanceManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Infrastructure.Repositories
{
    public class TransactionRepository(AppDbContext context) : ITransactionRepository
    {
        public async Task AddAsync(Transaction transaction)
            => await context.Transactions.AddAsync(transaction);

        public async Task<List<Transaction>> GetAllAsync()
            => await context.Transactions.AsNoTracking().Include(q => q.Category).ToListAsync();

        public async Task<Transaction> GetByIdAsync(Guid id)
            => await context.Transactions.AsNoTracking().Include(q => q.Category).FirstOrDefaultAsync(q => q.Id == id) ?? throw new EntityNotFoundInfraException("Transação não encontrada");

        public void Update(Transaction transaction)
            => context.Transactions.Update(transaction);

        public async Task<List<Transaction>> GetByFilterAsync(Expression<Func<Transaction, bool>> predicate)
            => await context.Transactions.Where(predicate).AsNoTracking().Include(q => q.Category).ToListAsync();
    }
}
