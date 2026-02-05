using Domain.Entities;
using Moq;

namespace Application.Tests.Services.CategoryAppServiceTests
{
    public class GetAllAsyncTests : CategoryAppServiceBaseTests
    {
        [Fact]
        public async Task WhenCategoriesExist_ShouldReturnAllCategories()
        {
            var categories = new List<Category>
            {
                Category.Create("Category 1", "Description 1"),
                Category.Create("Category 2", "Description 2")
            };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);
            
            var result = await _service.GetAllAsync();
            var first = result.First();
            var last = result.Last();

            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
            Assert.Equal(2, result.Count());
            Assert.Equal("Category 1", first.Name);
            Assert.Equal("Description 1", first.Description);
            Assert.Equal("Category 2", last.Name);
            Assert.Equal("Description 2", last.Description);
        }
    }
}
