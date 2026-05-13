using Finanza.Domain.Entities;
using Finanza.Domain.Exceptions;

namespace Finanza.Domain.Tests.Entities
{
    public class CategoryUpdateTests
    {
        [Fact]
        public void Update_WhenNameIsValid_ShouldChangeName()
        {
            var category = Category.Create("Original", "Description");

            category.Update("Updated", "New Description");

            Assert.Equal("Updated", category.Name);
            Assert.Equal("New Description", category.Description);
        }

        [Fact]
        public void Update_WhenDescriptionIsNull_ShouldClearDescription()
        {
            var category = Category.Create("Original", "Description");

            category.Update("Updated", null);

            Assert.Equal("Updated", category.Name);
            Assert.Null(category.Description);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_WhenNameIsEmpty_ShouldThrowDescriptionException(string invalidName)
        {
            var category = Category.Create("Original", null);

            Assert.Throws<DescriptionException>(() => category.Update(invalidName, null));
        }
    }
}
