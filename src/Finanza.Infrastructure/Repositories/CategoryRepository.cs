using Finanza.Domain.Entities;
using Finanza.Domain.Repositories;
using Finanza.Infrastructure.Data;
using Finanza.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Repositories
{
    public class CategoryRepository(TenantDbContext context) : ICategoryRepository
    {
        public async Task AddAsync(Category category)
            => await context.Categories.AddAsync(category);

        public Task UpdateAsync(Category category)
        {
            context.Categories.Update(category);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
            => await context.Categories.AsNoTracking().Include(q => q.Transactions).ToListAsync();

        public async Task<Category> GetByIdAsync(Guid id)
            => await context.Categories.AsNoTracking().Include(q => q.Transactions).FirstOrDefaultAsync(q => q.Id == id) ?? throw new EntityNotFoundInfraException("Category not found");
    }
}
