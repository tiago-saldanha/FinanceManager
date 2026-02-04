using Domain.Exceptions;

namespace Domain.ValueObjects
{
    public readonly record struct Description
    {
        public string Value { get; }

        public Description(string value)
        {
            value = value.Trim();

            if (string.IsNullOrEmpty(value))
                throw new DescriptionException();

            Value = value;
        }

        public static implicit operator string(Description description)
            => description.Value;
    }
}
