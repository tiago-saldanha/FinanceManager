using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects
{
    public class MoneyTests
    {
        [Fact]
        public void ShouldCreateMoneyWithValidValue()
        {
            var value = 100.50m;
            var money = new Money(value);
            decimal moneyValue = money;
            
            Assert.Equal(value, money.Value);
        }

        [Fact]
        public void ShouldCreateMoneyWithInvalidValue()
        {
            var invalidValue = -50.00m;

            Assert.Throws<TransactionAmountException>(() => new Money(invalidValue));
        }
    }
}
