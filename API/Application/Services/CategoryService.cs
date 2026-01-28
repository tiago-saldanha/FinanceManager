using API.Application.DTOs.Requests;
using API.Application.DTOs.Responses;
using API.Data;
using API.Domain.Entities;
using API.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace API.Application.Services
{
    public class CategoryService(AppDbContext context)
    {
        public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
        {
            var categories = await context.Categories.Include(q => q.Transactions).AsNoTracking().ToListAsync();
            return categories.Select(CategoryResponse.Create);
        }

        public async Task<CategoryResponse> GetByIdAsync(Guid id)
        {
            var category = await context.Categories
                .AsNoTracking()
                .Include(q => q.Transactions)
                .FirstOrDefaultAsync(q => q.Id == id) ?? throw new KeyNotFoundException("Category not found");
            
            return CategoryResponse.Create(category);
        }

        public async Task<CategoryResponse> CreateAsync(CategoryRequest request)
        {
            if (string.IsNullOrEmpty(request.Name)) throw new InvalidOperationException("Name is required");
            
            var category = new Category(Guid.NewGuid(), request.Name, request.Description);
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            
            return CategoryResponse.Create(category);
        }
    }
}
