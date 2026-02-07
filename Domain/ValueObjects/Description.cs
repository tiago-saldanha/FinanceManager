using Domain.Exceptions;

namespace Domain.ValueObjects
{
    public readonly record struct Description
    {
        public string Value { get; }

        public Description(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DescriptionException();

            Value = value.Trim();
        }

        public static implicit operator string(Description description)
            => description.Value;
    }
}
