using FinanceManager.Domain.Entities;
using FinanceManager.Application.DTOs.Requests;
using FinanceManager.Application.DTOs.Responses;
using FinanceManager.Domain.Repositories;
using FinanceManager.Application.Exceptions;
using FinanceManager.Application.Interfaces.Services;
using FinanceManager.Domain.Services;

namespace FinanceManager.Application.Services
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
