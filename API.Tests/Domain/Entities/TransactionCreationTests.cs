using API.Domain.Entities;
using API.Domain.Enums;
using API.Domain.Exceptions;

namespace API.Tests.Domain.Entities
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

            var message = Assert.Throws<TransactionAmountException>(() => Transaction.Create(description, amount, dueDate, type, categoryId, createdAt)).Message;

            Assert.Equal("O valor não pode ser negativo", message);
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

            var message = Assert.Throws<TransactionDateException>(() => Transaction.Create(description, amount, dueDate, type, categoryId, createdAt)).Message;

            Assert.Equal("A data de vencimento não pode ser anterior à data de criação", message);
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

            var message = Assert.Throws<DescriptionException>(() => Transaction.Create(description, amount, dueDate, type, categoryId, createdAt)).Message;

            Assert.Equal("A descrição deve ser informada", message);
        }
    }
}