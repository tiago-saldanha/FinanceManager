using Domain.Entities;
using Domain.Enums;

namespace Domain.Tests.Entities
{
    public class TransactionOverdueTests : TransactionBaseTests
    {
        [Fact]
        public void Create_WhenDueDateIsInTheFutureAndTypeIsRevenue_ShouldNotBeOverdue()
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
            Assert.False(sut.IsOverdue(Today));
        }

        [Fact]
        public void Create_WhenDueDateIsInThePastAndTypeIsRevenue_ShouldBeOverdue()
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
            Assert.True(sut.IsOverdue(Today));
        }

        [Fact]
        public void Create_WhenDueDateIsInTheFutureAndTypeIsExpense_ShouldNotBeOverdue()
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
            Assert.False(sut.IsOverdue(Today));
        }

        [Fact]
        public void Create_WhenDueDateIsInThePastAndTypeIsExpense_ShouldBeOverdue()
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
            Assert.True(sut.IsOverdue(Today));
        }
    }
}