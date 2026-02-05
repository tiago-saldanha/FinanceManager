using Application.DTOs.Requests;
using Domain.Entities;
using Moq;

namespace Application.Tests.Services.CategoryAppServiceTests
{
    public class CreateAsyncTests : CategoryAppServiceBaseTests
    {
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
