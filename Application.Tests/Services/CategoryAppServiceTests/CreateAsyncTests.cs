using Application.DTOs.Requests;
using Application.Exceptions;
using Domain.Entities;
using Moq;

namespace Application.Tests.Services.CategoryAppServiceTests
{
    public class CreateAsyncTests : CategoryAppServiceBaseTests
    {
        [Theory]
        [InlineData("Category 1", "Descrition 1")]
        [InlineData("Category 2", "")]
        [InlineData("Category 3", null)]
        public async Task CreateAsync_WhenRequestIsValid_ShouldCreateCategory(string name, string description)
        {
            var request = new CategoryRequest(name, description);

            var result = await _service.CreateAsync(request);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Description, result.Description);
        }

        [Theory]
        [InlineData("    ")]
        [InlineData("")]
        [InlineData(null)]
        public async Task CreateAsync_WhenNameIsInvalid_ShouldThrowCategoryNameAppException(string invalidName)
        {
            var request = new CategoryRequest(invalidName, "Description 1");
            
            await Assert.ThrowsAsync<CategoryNameAppException>(() => _service.CreateAsync(request));
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Never);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
