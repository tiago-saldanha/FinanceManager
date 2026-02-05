using Application.Services;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace Application.Tests.Services.CategoryAppServiceTests
{
    public class GetByIdAsyncTests
    {
        private readonly Mock<ICategoryRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly CategoryAppService _service;

        public GetByIdAsyncTests()
        {
            _repositoryMock = new Mock<ICategoryRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _service = new CategoryAppService(_repositoryMock.Object, _unitOfWork.Object);
        }

        [Fact]
        public async Task WhenCategoryIdIsProvided_ShouldReturnCategory()
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
