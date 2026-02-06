using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects
{
    public class MoneyTests
    {
        [Fact]
        public void Constructor_WhenValueIsValid_ShouldCreateMoney()
        {
            var value = 100.50m;
            var money = new Money(value);
            decimal moneyValue = money;
            
            Assert.Equal(value, money.Value);
        }

        [Fact]
        public void Constructor_WhenValueIsNegative_ShouldThrowTransactionAmountException()
        {
            var invalidValue = -50.00m;

            Assert.Throws<TransactionAmountException>(() => new Money(invalidValue));
        }
    }
}
