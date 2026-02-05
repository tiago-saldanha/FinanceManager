using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects
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
        }

        [Fact]
        public void ShouldNotCreateTransactionDates()
        {
            Assert.Throws<TransactionDateException>(() => new TransactionDates(Yesterday, Today));
        }
    }
}
