using Domain.Entities;
using Domain.Enums;
using Domain.Services;
using Domain.ValueObjects;

namespace Domain.Tests.Service
{
    public class CategoryTotalServiceTests
    {
        private readonly DateTime Yesterday = new(2025, 12, 31);
        private readonly DateTime Today = new(2026, 01, 01);
        private readonly DateTime Tomorrow = new(2026, 01, 02);

        [Fact]
        public void CalculateBalance_WhenCategoryHasTransactions_ShouldReturnCalculatedBalance()
        {
            List<Transaction> transactions = [
                Transaction.Create("Test", 100.0M, Today, TransactionType.Revenue, Guid.Empty, Yesterday),
                Transaction.Create("Test", 300.0M, Today, TransactionType.Revenue, Guid.Empty, Yesterday),
                Transaction.Create("Test", 400.0M, Tomorrow, TransactionType.Expense, Guid.Empty, Yesterday),
                Transaction.Create("Test", 500.0M, Tomorrow, TransactionType.Expense, Guid.Empty, Yesterday)
            ];
            var category = Category.Create("Name", "Description");
            category.Transactions = transactions;

            var sut = new CategoryTotalService();
            
            var result = sut.CalculateBalance(category);

            Assert.IsType<CategoryBalance>(result);
            Assert.Equal(400, result.Received);
            Assert.Equal(900, result.Spent);
            Assert.Equal(-500, result.Balance);
        }

        [Fact]
        public void CalculateBalance_WhenCategoryHasNoTransactions_ShouldReturnZeroBalance()
        {
            List<Transaction> transactions = [];
            var category = Category.Create("Name", "Description");
            category.Transactions = transactions;

            var sut = new CategoryTotalService();

            var result = sut.CalculateBalance(category);

            Assert.IsType<CategoryBalance>(result);
            Assert.Equal(0, result.Received);
            Assert.Equal(0, result.Spent);
            Assert.Equal(0, result.Balance);
        }
    }
}
