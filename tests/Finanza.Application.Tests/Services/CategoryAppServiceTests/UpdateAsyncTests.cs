using Finanza.Application.DTOs.Requests;
using Finanza.Application.Exceptions;
using Finanza.Domain.Entities;
using Moq;

namespace Finanza.Application.Tests.Services.CategoryAppServiceTests
{
    public class UpdateAsyncTests : CategoryAppServiceBaseTests
    {
        [Theory]
        [InlineData("New Name", "New Description")]
        [InlineData("New Name", null)]
        public async Task UpdateAsync_WhenRequestIsValid_ShouldUpdateCategory(string name, string? description)
        {
            var category = Category.Create("Original", "Original Description");
            var request = new CategoryRequest(name, description);
            _repositoryMock.Setup(r => r.GetByIdAsync(category.Id)).ReturnsAsync(category);

            var result = await _service.UpdateAsync(category.Id, request);

            _repositoryMock.Verify(r => r.GetByIdAsync(category.Id), Times.Once);
            _repositoryMock.Verify(r => r.UpdateAsync(category), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
            Assert.Equal(name, result.Name);
            Assert.Equal(description, result.Description);
        }

        [Theory]
        [InlineData("    ")]
        [InlineData("")]
        [InlineData(null)]
        public async Task UpdateAsync_WhenNameIsInvalid_ShouldThrowCategoryNameAppException(string? invalidName)
        {
            var request = new CategoryRequest(invalidName!, "Description");

            await Assert.ThrowsAsync<CategoryNameAppException>(() => _service.UpdateAsync(Guid.NewGuid(), request));
            _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Never);
            _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
