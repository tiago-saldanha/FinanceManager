using API.Domain.Exceptions;
using API.Domain.ValueObjects;

namespace API.Tests.Domain.ValueObjects
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
            var message = Assert.Throws<TransactionAmountException>(() => new Money(invalidValue)).Message;
            Assert.Equal("O valor não pode ser negativo", message);
        }
    }
}
