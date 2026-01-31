using API.Domain.Exceptions;
using API.Domain.ValueObjects;

namespace API.Tests.Domain.ValueObjects
{
    public class DescriptionTests
    {
        [Fact]
        public void ShouldCreateDescriptionWithValidValue()
        {
            var validDescription = "Gastos com internet";
            var description = new Description(validDescription);
            string descriptionString = description;
            Assert.Equal(validDescription, description.Value);
            Assert.NotNull(description);
            Assert.IsType<string>(description.Value);
            Assert.False(string.IsNullOrEmpty(description.Value));
            Assert.Equal(validDescription, description.Value);
            Assert.IsType<string>(descriptionString);
        }

        [Fact]
        public void ShouldNotCreateDescriptionWithInvalidValue()
        {
            var invalidDescription = string.Empty;
            var message = Assert.Throws<DescriptionException>(() => new Description(invalidDescription)).Message;
            Assert.Equal("A descrição deve ser informada", message);
            Assert.NotNull(message);
        }
    }
}
