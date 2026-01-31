using API.Domain.Exceptions;
using API.Domain.ValueObjects;

namespace API.Tests.Domain.ValueObjects
{
    public class MoneyTests
    {
        [Fact]
        public void ShouldCreateMoneyWithValidValue()
        {
            var validValue = 100.50m;
            var money = new Money(validValue);
            decimal modeyDecial = money;
            Assert.Equal(validValue, money.Value);
            Assert.NotNull(money);
            Assert.IsType<decimal>(money.Value);
            Assert.True(money.Value >= 0);
            Assert.Equal(validValue, money.Value);
            Assert.IsType<decimal>(modeyDecial);
        }

        [Fact]
        public void ShouldCreateMoneyWithInvalidValue()
        {
            var invalidValue = -50.00m;
            var message = Assert.Throws<TransactionAmountException>(() => new Money(invalidValue)).Message;
            Assert.Equal("O valor não pode ser negativo", message);
            Assert.NotNull(message);
        }
    }
}
