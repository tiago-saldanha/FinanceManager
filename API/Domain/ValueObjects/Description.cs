using API.Domain.Exceptions;

namespace API.Domain.ValueObjects
{
    public sealed record class Description
    {
        public string Value { get; }

        public Description(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new DescriptionException();

            Value = value;
        }

        public static implicit operator string(Description description)
            => description.Value;
    }
}
