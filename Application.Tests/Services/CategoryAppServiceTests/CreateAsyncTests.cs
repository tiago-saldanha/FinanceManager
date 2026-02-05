using Application.DTOs.Requests;
using Application.Services;
using Domain.Entities;
using Domain.Repositories;
using Moq;

namespace Application.Tests.Services.CategoryAppServiceTests
{
    public class CreateAsyncTests
    {
        private readonly Mock<ICategoryRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly CategoryAppService _service;

        public CreateAsyncTests()
        {
            _repositoryMock = new Mock<ICategoryRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _service = new CategoryAppService(_repositoryMock.Object, _unitOfWork.Object);
        }

        [Fact]
        public async Task WhenRequestIsValid_ShouldCreateCategory()
        {
            var request = new CategoryRequest("Category 1", "Description 1");

            var result = await _service.CreateAsync(request);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Description, result.Description);
        }

        [Fact]
        public async Task WhenNameIsEmpty_ShouldThrowException()
        {
            var request = new CategoryRequest("", "Description 1");
            
            await Assert.ThrowsAsync<Exceptions.CategoryNameAppException>(() => _service.CreateAsync(request));

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Never);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
