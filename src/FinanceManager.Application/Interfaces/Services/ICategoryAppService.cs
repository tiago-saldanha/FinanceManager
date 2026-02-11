using Application.DTOs.Requests;
using Application.DTOs.Responses;

namespace Application.Interfaces.Services
{
    public interface ICategoryAppService
    {
        Task<IEnumerable<CategoryResponse>> GetAllAsync();
        Task<CategoryResponse> GetByIdAsync(Guid id);
        Task<CategoryResponse> CreateAsync(CategoryRequest request);
    }
}
