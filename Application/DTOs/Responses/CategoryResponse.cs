using Domain.Entities;
using Domain.Services;

namespace Application.DTOs.Responses
{
    public class CategoryResponse
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public CategoryTotal Total { get; private set; }

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
