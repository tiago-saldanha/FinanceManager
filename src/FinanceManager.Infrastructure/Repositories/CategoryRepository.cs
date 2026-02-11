using FinanceManager.Domain.Entities;
using FinanceManager.Domain.Repositories;
using FinanceManager.Infrastructure.Data;
using FinanceManager.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Infrastructure.Repositories
{
    public class CategoryRepository(AppDbContext context) : ICategoryRepository
    {
        public async Task AddAsync(Category category)
            => await context.Categories.AddAsync(category);

        public async Task<IEnumerable<Category>> GetAllAsync()
            => await context.Categories.AsNoTracking().Include(q => q.Transactions).ToListAsync();

        public async Task<Category> GetByIdAsync(Guid id)
            => await context.Categories.AsNoTracking().Include(q => q.Transactions).FirstOrDefaultAsync(q => q.Id == id) ?? throw new EntityNotFoundInfraException("Category not found");
    }
}
