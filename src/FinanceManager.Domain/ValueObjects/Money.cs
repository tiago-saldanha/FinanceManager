using Domain.Exceptions;

namespace Domain.ValueObjects
{
    public readonly record struct Money
    {
        public decimal Value { get; }

        public Money(decimal value)
        {
            if (value < 0)
                throw new TransactionAmountException();

            Value = value;
        }

        public static implicit operator decimal(Money money) 
            => money.Value;
    }
}
