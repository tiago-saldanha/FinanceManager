using FinanceManager.Domain.Entities;
using FinanceManager.Domain.Enums;
using FinanceManager.Domain.Repositories;
using FinanceManager.Infrastructure.Data;
using FinanceManager.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        public void Remove(Transaction transaction)
            => context.Transactions.Remove(transaction);

        public async Task<List<Transaction>> SearchAsync(string? search, TransactionStatus? status, TransactionType? type, DateTime? startDate, DateTime? endDate)
        {
            var query = context.Transactions.AsNoTracking().Include(q => q.Category).AsQueryable();

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (type.HasValue)
                query = query.Where(t => t.Type == type.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(t => EF.Functions.Like((string)(object)t.Description, $"%{search}%"));

            if (startDate.HasValue)
                query = query.Where(t => t.Dates.DueDate.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(t => t.Dates.DueDate.Date <= endDate.Value.Date);

            return await query.OrderByDescending(t => t.Dates.DueDate).ToListAsync();
        }
    }
}