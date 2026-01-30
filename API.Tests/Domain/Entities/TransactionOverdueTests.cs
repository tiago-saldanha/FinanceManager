using API.Domain.Entities;
using API.Domain.Enums;

namespace API.Tests.Domain.Entities
{
    public class TransactionOverdueTests
    {
        private static readonly DateTime Yesterday = DateTime.Now.AddDays(-1);
        private static readonly DateTime Today = DateTime.Now.AddDays(0);
        private static readonly DateTime Tomorrow = DateTime.Now.AddDays(1);

        [Fact]
        public void ShouldCreateTransactionRevenue()
        {
            var description = "Test";
            var amount = 100.0M;
            var dueDate = Tomorrow;
            var type = TransactionType.Revenue;
            var categoryId = Guid.NewGuid();
            var createdAt = Tomorrow;

            var sut = Transaction.Create(description, amount, dueDate, type, categoryId, createdAt);

            Assert.NotNull(sut);
            Assert.IsType<Guid>(sut.Id);
            Assert.NotEqual(Guid.Empty, sut.Id);
            Assert.Equal(description, sut.Description);
            Assert.Equal(amount, sut.Amount);
            Assert.Equal(dueDate, sut.Dates.DueDate);
            Assert.True(sut.Type == TransactionType.Revenue);
            Assert.True(sut.Status == TransactionStatus.Pending);
            Assert.Equal(sut.CategoryId, categoryId);
            Assert.Equal(createdAt, sut.Dates.CreatedAt);
            Assert.Null(sut.PaymentDate);
            Assert.False(sut.IsOverdue);
        }

        [Fact]
        public void ShouldCreateTransactionRevenueOverDue()
        {
            var description = "Test";
            var amount = 100.0M;
            var dueDate = Yesterday;
            var type = TransactionType.Revenue;
            var categoryId = Guid.NewGuid();
            var createdAt = Yesterday;

            var sut = Transaction.Create(description, amount, dueDate, type, categoryId, createdAt);

            Assert.NotNull(sut);
            Assert.IsType<Guid>(sut.Id);
            Assert.NotEqual(Guid.Empty, sut.Id);
            Assert.Equal(description, sut.Description);
            Assert.Equal(amount, sut.Amount);
            Assert.Equal(dueDate, sut.Dates.DueDate);
            Assert.True(sut.Type == TransactionType.Revenue);
            Assert.True(sut.Status == TransactionStatus.Pending);
            Assert.Equal(sut.CategoryId, categoryId);
            Assert.Equal(createdAt, sut.Dates.CreatedAt);
            Assert.Null(sut.PaymentDate);
            Assert.True(sut.IsOverdue);
        }

        [Fact]
        public void ShouldCreateTransactionExpense()
        {
            var description = "Test";
            var amount = 100.0M;
            var dueDate = Tomorrow;
            var type = TransactionType.Expense;
            var categoryId = Guid.NewGuid();
            var createdAt = Tomorrow;

            var sut = Transaction.Create(description, amount, dueDate, type, categoryId, createdAt);

            Assert.NotNull(sut);
            Assert.IsType<Guid>(sut.Id);
            Assert.NotEqual(Guid.Empty, sut.Id);
            Assert.Equal(description, sut.Description);
            Assert.Equal(amount, sut.Amount);
            Assert.Equal(dueDate, sut.Dates.DueDate);
            Assert.True(sut.Type == TransactionType.Expense);
            Assert.True(sut.Status == TransactionStatus.Pending);
            Assert.Equal(sut.CategoryId, categoryId);
            Assert.Equal(createdAt, sut.Dates.CreatedAt);
            Assert.Null(sut.PaymentDate);
            Assert.False(sut.IsOverdue);
        }

        [Fact]
        public void ShouldCreateTransactionExpenseOverDue()
        {
            var description = "Test";
            var amount = 100.0M;
            var dueDate = Yesterday;
            var type = TransactionType.Expense;
            var categoryId = Guid.NewGuid();
            var createdAt = Yesterday;

            var sut = Transaction.Create(description, amount, dueDate, type, categoryId, createdAt);

            Assert.NotNull(sut);
            Assert.IsType<Guid>(sut.Id);
            Assert.NotEqual(Guid.Empty, sut.Id);
            Assert.Equal(description, sut.Description);
            Assert.Equal(amount, sut.Amount);
            Assert.Equal(dueDate, sut.Dates.DueDate);
            Assert.True(sut.Type == TransactionType.Expense);
            Assert.True(sut.Status == TransactionStatus.Pending);
            Assert.Equal(sut.CategoryId, categoryId);
            Assert.Equal(createdAt, sut.Dates.CreatedAt);
            Assert.Null(sut.PaymentDate);
            Assert.True(sut.IsOverdue);
        }
    }
}