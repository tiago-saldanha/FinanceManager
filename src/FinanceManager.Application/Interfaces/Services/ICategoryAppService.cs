using FinanceManager.Application.DTOs.Requests;
using FinanceManager.Application.DTOs.Responses;

namespace FinanceManager.Application.Interfaces.Services
{
    public interface ICategoryAppService
    {
        Task<IEnumerable<CategoryResponse>> GetAllAsync();
        Task<CategoryResponse> GetByIdAsync(Guid id);
        Task<CategoryResponse> CreateAsync(CategoryRequest request);
    }
}
