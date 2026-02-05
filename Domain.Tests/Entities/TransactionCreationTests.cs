using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Tests.Entities
{
    public class TransactionCreationTests
    {
        private static readonly DateTime Yesterday = new(2025, 12, 31);
        private static readonly DateTime Today = new(2026, 01, 01);
        private static readonly DateTime Tomorrow = new(2026, 01, 02);

        [Fact]
        public void ShouldCreateTransactionRevenue()
        {
            var description = "Test";
            var amount = 100.0M;
            var dueDate = Yesterday;
            var type = TransactionType.Revenue;
            var categoryId = Guid.NewGuid();
            var createdAt = Yesterday;

            var sut = Transaction.Create(description, amount, dueDate, type, categoryId, createdAt);

            Assert.NotNull(sut);
            Assert.NotEqual(Guid.Empty, sut.Id);
            Assert.Equal(description, sut.Description);
            Assert.Equal(amount, sut.Amount);
            Assert.Equal(dueDate, sut.Dates.DueDate);
            Assert.True(sut.Type == TransactionType.Revenue);
            Assert.True(sut.Status == TransactionStatus.Pending);
            Assert.Equal(sut.CategoryId, categoryId);
            Assert.Equal(createdAt, sut.Dates.CreatedAt);
            Assert.Null(sut.PaymentDate);
            Assert.True(sut.IsOverdue(Today));
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
            Assert.NotEqual(Guid.Empty, sut.Id);
            Assert.Equal(description, sut.Description);
            Assert.Equal(amount, sut.Amount);
            Assert.Equal(dueDate, sut.Dates.DueDate);
            Assert.True(sut.Type == TransactionType.Expense);
            Assert.True(sut.Status == TransactionStatus.Pending);
            Assert.Equal(sut.CategoryId, categoryId);
            Assert.Equal(createdAt, sut.Dates.CreatedAt);
            Assert.Null(sut.PaymentDate);
            Assert.False(sut.IsOverdue(Today));
        }

        [Fact]
        public void ShouldNotCreateTransactionWithAmountLessThenZero()
        {
            var description = "Test";
            var amount = -100.0M;
            var dueDate = Today;
            var type = TransactionType.Revenue;
            var categoryId = Guid.NewGuid();
            var createdAt = Today;

            Assert.Throws<TransactionAmountException>(() => Transaction.Create(description, amount, dueDate, type, categoryId, createdAt));
        }

        [Fact]
        public void ShouldNotCreateTransactionWithCreatedAtLessThenDueDate()
        {
            var description = "Test";
            var amount = 100.0M;
            var dueDate = Yesterday;
            var type = TransactionType.Revenue;
            var categoryId = Guid.NewGuid();
            var createdAt = Today;

            Assert.Throws<TransactionDateException>(() => Transaction.Create(description, amount, dueDate, type, categoryId, createdAt));
        }

        [Fact]
        public void ShouldNotCreateTransactionWithoutDescription()
        {
            var description = string.Empty;
            var amount = 100.0M;
            var dueDate = Today;
            var type = TransactionType.Revenue;
            var categoryId = Guid.NewGuid();
            var createdAt = Today;

            Assert.Throws<DescriptionException>(() => Transaction.Create(description, amount, dueDate, type, categoryId, createdAt));
        }
    }
}