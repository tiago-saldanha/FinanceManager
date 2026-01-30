using API.Domain.Exceptions;

namespace API.Domain.ValueObjects
{
    public sealed record class TransactionDescription
    {
        public string Value { get; }

        public TransactionDescription(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new TransactionException("A descrição deve ser informada");

            Value = value;
        }

        public static implicit operator string(TransactionDescription description)
            => description.Value;
    }
}
