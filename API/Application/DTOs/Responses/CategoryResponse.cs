using API.Domain.Entities;
using API.Domain.Enums;

namespace API.Application.DTOs.Responses
{
    public class CategoryResponse
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public CategoryTotal Total { get; private set; }

        public static CategoryResponse Create(Category category)
        {
            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category?.Description,
                Total = CalculateTotal(category!)
            };
        }

        private static CategoryTotal CalculateTotal(Category category)
        {
            var received = category.Transactions
                .Where(t => t.Type == TransactionType.Revenue)
                .Sum(t => t.Amount);

            var spent = category.Transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount);

            var total = received - spent;

            return new CategoryTotal(received, spent, total);
        }
    }
}
