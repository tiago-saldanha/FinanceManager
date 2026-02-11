using FinanceManager.Domain.Entities;
using Moq;

namespace FinanceManager.Application.Tests.Services.CategoryAppServiceTests
{
    public class GetByIdAsyncTests : CategoryAppServiceBaseTests
    {
        [Fact]
        public async Task GetByIdAsync_WhenCategoryIdIsProvided_ShouldReturnCategory()
        {
            var category = Category.Create("Category 1", "Description 1");
            _repositoryMock.Setup(r => r.GetByIdAsync(category.Id)).ReturnsAsync(category);

            var result = await _service.GetByIdAsync(category.Id);

            _repositoryMock.Verify(r => r.GetByIdAsync(category.Id), Times.Once);
            Assert.Equal(category.Id, result.Id);
            Assert.Equal(category.Name, result.Name);
            Assert.Equal(category.Description, result.Description);
        }
    }
}
