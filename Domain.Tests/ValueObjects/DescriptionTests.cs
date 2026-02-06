using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects
{
    public class DescriptionTests
    {
        [Fact]
        public void Constructor_WhenValueIsValid_ShouldCreateDescription()
        {
            var validDescription = "Gastos com internet";
            var description = new Description(validDescription);
            
            string descriptionString = description;
            Assert.Equal(validDescription, description.Value);
        }

        [Fact]
        public void Constructor_WhenValueIsEmpty_ShouldThrowDescriptionException()
        {
            var invalidDescription = string.Empty;
            
            Assert.Throws<DescriptionException>(() => new Description(invalidDescription));
        }
    }
}
