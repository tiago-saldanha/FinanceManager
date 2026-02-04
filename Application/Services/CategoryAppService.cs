using Domain.Entities;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Repositories;
using Application.Exceptions;

namespace Application.Services
{
    public class CategoryAppService(ICategoryRepository repository, IUnitOfWork unitOfWork) : ICategoryAppService
    {
        public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
        {
            var categories = await repository.GetAllAsync();
            return categories.Select(CategoryResponse.Create);
        }

        public async Task<CategoryResponse> GetByIdAsync(Guid id)
        {
            var category = await repository.GetByIdAsync(id);
            return CategoryResponse.Create(category);
        }

        public async Task<CategoryResponse> CreateAsync(CategoryRequest request)
        {
            if (string.IsNullOrEmpty(request.Name)) throw new CategoryNameAppException();
            var category = Category.Create(request.Name, request.Description);
            await repository.AddAsync(category);
            await unitOfWork.CommitAsync();
            return CategoryResponse.Create(category);
        }
    }
}
