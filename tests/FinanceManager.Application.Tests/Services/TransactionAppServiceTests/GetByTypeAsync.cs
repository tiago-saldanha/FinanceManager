using System.Linq.Expressions;
using Application.Enums;
using Domain.Entities;
using Domain.Enums;
using Moq;

namespace Application.Tests.Services.TransactionAppServiceTests
{
    public class GetByTypeAsync : TransactionAppServiceBaseTests
    {
        [Fact]
        public async Task GetByTypeAsync_WhenTypeIsProvided_ShouldReturnMatchingTransactions()
        {
            var transactions = new List<Transaction>()
            {
                Transaction.Create("Description 1", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Today),
                Transaction.Create("Description 2", 200, Tomorrow, TransactionType.Revenue, Guid.Empty, Today)
            };
            _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<Expression<Func<Transaction, bool>>>())).ReturnsAsync(transactions);

            var result = await _service.GetByTypeAsync(TransactionTypeDto.Revenue);

            _repositoryMock.Verify(r => r.GetByFilterAsync(It.IsAny<Expression<Func<Transaction, bool>>>()), Times.Once);
            var first = result.First();
            var last = result.Last();
            Assert.Equal(2, result.Count());
            Assert.Equal("Description 1", first.Description);
            Assert.Equal(TransactionTypeDto.Revenue.ToString(), first.Type);
            Assert.Equal(100, first.Amount);
            Assert.Equal("Description 2", last.Description);
            Assert.Equal(200, last.Amount);
            Assert.Equal(TransactionTypeDto.Revenue.ToString(), last.Type);
        }
    }
}
