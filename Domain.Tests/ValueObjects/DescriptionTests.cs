using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects
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
        }

        [Fact]
        public void ShouldNotCreateDescriptionWithInvalidValue()
        {
            var invalidDescription = string.Empty;
            
            Assert.Throws<DescriptionException>(() => new Description(invalidDescription));
        }
    }
}
