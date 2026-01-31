using API.Domain.Exceptions;
using API.Domain.ValueObjects;

namespace API.Tests.Domain.ValueObjects
{
    public class TransactionDatesTests
    {
        private readonly static DateTime Yesterday = new(2025, 12, 31);
        private readonly static DateTime Today = new(2026, 01, 01);
        private readonly static DateTime Tomorrow = new(2026, 01, 02);

        [Fact]
        public void ShouldCreateTransactionDates()
        {
            var transactionDates = new TransactionDates(Today, Yesterday);
            Assert.Equal(Today, transactionDates.DueDate);
            Assert.Equal(Yesterday, transactionDates.CreatedAt);
            Assert.NotNull(transactionDates);
        }

        [Fact]
        public void ShouldNotCreateTransactionDates()
        {
            var message = Assert.Throws<TransactionDateException>(() => new TransactionDates(Yesterday, Today)).Message;
            Assert.Equal("A data de vencimento não pode ser anterior à data de criação", message);
        }
    }
}
