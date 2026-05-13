using Finanza.Application.DTOs.Requests;
using Finanza.Application.DTOs.Responses;

namespace Finanza.Application.Interfaces.Services
{
    public interface ICategoryAppService
    {
        Task<IEnumerable<CategoryResponse>> GetAllAsync();
        Task<CategoryResponse> GetByIdAsync(Guid id);
        Task<CategoryResponse> CreateAsync(CategoryRequest request);
        Task<CategoryResponse> UpdateAsync(Guid id, CategoryRequest request);
    }
}
