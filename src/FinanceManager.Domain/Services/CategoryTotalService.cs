using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Services
{
    public class CategoryTotalService
    {
        public CategoryBalance CalculateBalance(Category category)
        {
            var received = category.Transactions
                .Where(t => t.Type == TransactionType.Revenue)
                .Sum(t => t.Amount.Value);

            var spent = category.Transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Amount.Value);

            var balance = received - spent;

            return new CategoryBalance(received, spent, balance);
        }
    }
}
