using Finanza.Domain.Entities;
using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;
using Finanza.Domain.Repositories;
using Finanza.Application.Exceptions;
using Finanza.Application.Interfaces.Services;
using Finanza.Domain.Services;

namespace Finanza.Application.Services
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

        public async Task<CategoryResponse> UpdateAsync(Guid id, CategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new CategoryNameAppException();
            var category = await repository.GetByIdAsync(id);
            category.Update(request.Name, request.Description);
            await repository.UpdateAsync(category);
            await unitOfWork.CommitAsync();
            var domainService = new CategoryTotalService();
            return CategoryResponse.Create(category, domainService);
        }
    }
}
