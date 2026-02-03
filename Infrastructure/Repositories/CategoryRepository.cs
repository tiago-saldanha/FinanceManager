using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
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
