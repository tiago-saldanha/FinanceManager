using Domain.Entities;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Domain.Repositories;
using Application.Exceptions;
using Application.Interfaces.Services;
using Domain.Services;

namespace Application.Services
{
    public class CategoryAppService(ICategoryRepository repository, IUnitOfWork unitOfWork) : ICategoryAppService
    {
        public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
        {
            var categories = await repository.GetAllAsync();
            var domainService = new CategoryTotalService();
            return categories.Select(x => CategoryResponse.Create(x, domainService));
        }

        public async Task<CategoryResponse> GetByIdAsync(Guid id)
        {
            var category = await repository.GetByIdAsync(id);
            var domainService = new CategoryTotalService();
            return CategoryResponse.Create(category, domainService);
        }

        public async Task<CategoryResponse> CreateAsync(CategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new CategoryNameAppException();
            var category = Category.Create(request.Name, request.Description);
            await repository.AddAsync(category);
            await unitOfWork.CommitAsync();
            var domainService = new CategoryTotalService();
            return CategoryResponse.Create(category, domainService);
        }
    }
}
