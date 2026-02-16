using FinanceManager.Domain.Entities;
using FinanceManager.Domain.Services;

namespace FinanceManager.Application.DTOs.Responses
{
    public class CategoryResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public string? Description { get; init; }
        public CategoryTotal Total { get; init; } = default!;

        public static CategoryResponse Create(Category category, CategoryTotalService domainService)
        {
            var categoryBalance = domainService.CalculateBalance(category);
            var categoryTotal = new CategoryTotal(categoryBalance.Received, categoryBalance.Spent, categoryBalance.Balance);
            
            var response = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Total = categoryTotal
            };

            return response;
        }
    }
}
