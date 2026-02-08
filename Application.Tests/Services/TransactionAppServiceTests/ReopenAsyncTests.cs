using Application.DTOs.Responses;
using Application.Enums;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Moq;

namespace Application.Tests.Services.TransactionAppServiceTests
{
    public class ReopenAsyncTests : TransactionAppServiceBaseTests
    {
        [Fact]
        public async Task ReopenAsync_WhenTransactionIsPaid_ShouldMarkAsPending()
        {
            var transaction = Transaction.Create("Description 1", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Today);
            transaction.Pay(Today);
            _repositoryMock.Setup(r => r.GetByIdAsync(transaction.Id)).ReturnsAsync(transaction);
            
            var result = await _service.ReopenAsync(transaction.Id);

            _repositoryMock.Verify(r => r.Update(transaction), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _dispatcherMock.Verify(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>()), Times.Once);
            Assert.IsType<TransactionResponse>(result);
            Assert.Equal(transaction.Id, result.Id);
            Assert.Null(result.PaymentDate);
            Assert.Equal(TransactionStatusDto.Pending.ToString(), result.Status);
        }

        [Fact]
        public async Task ReopenAsync_WhenTransactionIsNotPaid_ShouldThrowTransactionReopenException()
        {
            var transaction = Transaction.Create("Description 1", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Today);
            _repositoryMock.Setup(r => r.GetByIdAsync(transaction.Id)).ReturnsAsync(transaction);

            await Assert.ThrowsAsync<TransactionReopenException>(() => _service.ReopenAsync(transaction.Id));
            _dispatcherMock.Verify(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>()), Times.Never);
            Assert.Null(transaction.PaymentDate);
            Assert.Equal(TransactionStatus.Pending, transaction.Status);
        }
    }
}
