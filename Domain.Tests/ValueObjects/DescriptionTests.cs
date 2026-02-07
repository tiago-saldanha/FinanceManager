using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Tests.ValueObjects
{
    public class DescriptionTests
    {
        [Theory]
        [InlineData("Gastos com internet")]
        [InlineData("Gastos com internet ")]
        [InlineData(" Gastos com internet ")]
        public void Constructor_WhenValueIsValid_ShouldCreateDescription(string validDescription)
        {
            var description = new Description(validDescription);
            
            string descriptionString = description;
            Assert.Equal(validDescription.Trim(), description.Value);
        }

        [Theory]
        [InlineData("    ")]
        [InlineData("")]
        [InlineData(null)]
        public void Constructor_WhenValueIsEmpty_ShouldThrowDescriptionException(string invalidDescription)
        {
            Assert.Throws<DescriptionException>(() => new Description(invalidDescription));
        }
    }
}
