using System.Linq.Expressions;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Exceptions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TransactionRepository(AppDbContext context) : ITransactionRepository
    {
        public async Task CreateAsync(Transaction transaction)
            => await context.Transactions.AddAsync(transaction);

        public async Task<List<Transaction>> GetAllAsync()
            => await context.Transactions.Include(q => q.Category).AsNoTracking().ToListAsync();

        public async Task<Transaction> GetByIdAsync(Guid id)
            => await context.Transactions.Include(q => q.Category).AsNoTracking().FirstOrDefaultAsync(q => q.Id == id) ?? throw new EntityNotFoundInfraException("Transação não encontrada");

        public void Update(Transaction transaction)
            => context.Transactions.Update(transaction);

        public async Task<List<Transaction>> GetByFilterAsync(Expression<Func<Transaction, bool>> predicate)
            => await context.Transactions.Where(predicate).Include(q => q.Category).AsNoTracking().ToListAsync();
    }
}
