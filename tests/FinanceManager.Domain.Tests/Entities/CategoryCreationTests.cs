using Domain.Entities;
using Domain.Exceptions;

namespace Domain.Tests.Entities
{
    public class CategoryCreationTests
    {
        [Fact]
        public void Create_WhenNameAndDescriptionAreValid_ShouldCreateCategory()
        {
            var name = "Electronics";
            var description = "All kinds of electronic devices";

            var category = Category.Create(name, description);

            Assert.NotNull(category);
            Assert.NotEqual(Guid.Empty, category.Id);
            Assert.Equal(name, category.Name);
            Assert.Equal(description, category.Description);
        }

        [Fact]
        public void Create_WhenDescriptionIsNull_ShouldCreateCategory()
        {
            var name = "Books";

            var category = Category.Create(name, null);

            Assert.NotNull(category);
            Assert.IsType<Category>(category);
            Assert.NotEqual(Guid.Empty, category.Id);
            Assert.Equal(name, category.Name);
            Assert.Null(category.Description);
        }

        [Fact]
        public void Create_WhenNameIsEmpty_ShouldThrowDescriptionException()
        {
            var name = "   ";
            
            Assert.Throws<DescriptionException>(() => Category.Create(name, null));
        }
    }
}
