using Application.DTOs.Responses;
using Domain.Entities;
using Domain.Enums;

namespace Application.Tests.DTOs.Responses
{
    public class TransactionResponseTests
    {
        protected readonly DateTime Today = new(2026, 01, 01);
        protected readonly DateTime Tomorrow = new(2026, 01, 02);

        [Fact]
        public void Create_WhenTransactionIsValid_ShouldReturnTransactionResponse()
        {
            var transaction = Transaction.Create("Description", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Today);

            var result = TransactionResponse.Create(transaction);

            Assert.NotNull(result);
            Assert.Equal(transaction.Description, result.Description);
            Assert.Equal(transaction.Amount, result.Amount);
            Assert.Equal(transaction.Dates.DueDate, result.DueDate);
            Assert.Equal(transaction.Type.ToString(), result.Type.ToString());
            Assert.Equal(transaction.Dates.CreatedAt, result.CreatedAt);
        }
    }
}
