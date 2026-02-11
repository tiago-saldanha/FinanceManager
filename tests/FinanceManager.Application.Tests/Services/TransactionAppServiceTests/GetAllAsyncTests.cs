using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Application.Tests.Services.TransactionAppServiceTests
{
    public class GetAllAsyncTests : TransactionAppServiceBaseTests
    {
        [Fact]
        public async Task GetAllAsync_WhenTransactionsExist_ShouldReturnAllTransactions()
        {
            var transactions = new List<Transaction>()
            {
                Transaction.Create("Description 1", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Yesterday),
                Transaction.Create("Description 2", 200, Tomorrow, TransactionType.Expense, Guid.Empty, Today),
                Transaction.Create("Description 3", 300, Tomorrow, TransactionType.Revenue, Guid.Empty, Tomorrow),
                Transaction.Create("Description 4", 400, Tomorrow, TransactionType.Revenue, Guid.Empty, Tomorrow),
            };
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(transactions);

            var result = await _service.GetAllAsync();

            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
            Assert.Equal(4, result.Count());
            Assert.Equal("Description 1", result.First().Description);
            Assert.Equal(100, result.First().Amount);
            Assert.Equal("Description 4", result.Last().Description);
            Assert.Equal(400, result.Last().Amount);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoTransactionsExist_ShouldReturnEmptyList()
        {
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

            var result = await _service.GetAllAsync();

            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
            Assert.False(result.Any());
            Assert.Empty(result);
        }
    }
}
