using FinanceManager.Domain.ValueObjects;

namespace FinanceManager.Domain.Tests.ValueObjects
{
    public class TransactionDatesTests
    {
        private readonly static DateTime Yesterday = new(2025, 12, 31);
        private readonly static DateTime Today = new(2026, 01, 01);

        [Fact]
        public void Constructor_WhenDatesAreValid_ShouldCreateTransactionDates()
        {
            var transactionDates = new TransactionDates(Today, Yesterday);
            
            Assert.Equal(Today, transactionDates.DueDate);
            Assert.Equal(Yesterday, transactionDates.CreatedAt);
        }
    }
}
