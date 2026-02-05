using Application.DTOs.Requests;
using Application.Services;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace Application.Tests.Services
{
    public class CategoryAppServiceTests
    {
        private readonly Mock<ICategoryRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly CategoryAppService _service;

        public CategoryAppServiceTests()
        {
            _repositoryMock = new Mock<ICategoryRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _service = new CategoryAppService(_repositoryMock.Object, _unitOfWork.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCategories()
        {
            var categories = new List<Domain.Entities.Category>
            {
                Domain.Entities.Category.Create("Category 1", "Description 1"),
                Domain.Entities.Category.Create("Category 2", "Description 2")
            };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);
            
            var result = await _service.GetAllAsync();

            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);

            var first = result.First();
            var last = result.Last();

            Assert.Equal(2, result.Count());
            Assert.Equal("Category 1", first.Name);
            Assert.Equal("Description 1", first.Description);
            Assert.Equal("Category 2", last.Name);
            Assert.Equal("Description 2", last.Description);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCategoryResponse()
        {
            var category = Category.Create("Category 1", "Description 1");

            _repositoryMock.Setup(r => r.GetByIdAsync(category.Id)).ReturnsAsync(category);

            var result = await _service.GetByIdAsync(category.Id);

            _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            
            Assert.Equal(category.Id, result.Id);
            Assert.Equal(category.Name, result.Name);
            Assert.Equal(category.Description, result.Description);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateCategory()
        {
            var request = new CategoryRequest("Category 1", "Description 1");

            var result = await _service.CreateAsync(request);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);

            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Description, result.Description);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenNameIsEmpty()
        {
            var request = new CategoryRequest("", "Description 1");
            await Assert.ThrowsAsync<Application.Exceptions.CategoryNameAppException>(async () => await _service.CreateAsync(request));
            
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Never);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
