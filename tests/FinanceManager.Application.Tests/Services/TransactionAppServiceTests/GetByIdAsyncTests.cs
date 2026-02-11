using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Application.Tests.Services.TransactionAppServiceTests
{
    public class GetByIdAsyncTests : TransactionAppServiceBaseTests
    {
        [Fact]
        public async Task GetByIdAsync_WhenTransactionExists_ShouldReturnTransaction()
        {
            var transaction = Transaction.Create("Description", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Today);
            _repositoryMock.Setup(r => r.GetByIdAsync(transaction.Id)).ReturnsAsync(transaction);
            
            var result = await _service.GetByIdAsync(transaction.Id);
            
            _repositoryMock.Verify(r => r.GetByIdAsync(transaction.Id), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(transaction.Id, result.Id);
            Assert.Equal(transaction.Description, result.Description);
            Assert.Equal(transaction.Amount, result.Amount);
            Assert.Equal(transaction.Dates.DueDate, result.DueDate);
            Assert.Equal(transaction.Dates.CreatedAt, result.CreatedAt);
            Assert.Equal(TransactionType.Revenue.ToString(), result.Type);
            Assert.Equal(TransactionStatus.Pending.ToString(), result.Status);
            Assert.NotNull(result.CategoryName);
        }
    }
}
