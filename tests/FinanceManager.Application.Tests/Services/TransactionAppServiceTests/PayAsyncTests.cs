using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Enums;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Moq;

namespace Application.Tests.Services.TransactionAppServiceTests
{
    public class PayAsyncTests : TransactionAppServiceBaseTests
    {
        [Fact]
        public async Task PayAsync_WhenTransactionIsPending_ShouldMarkAsPaid()
        {
            var request = new PayTransactionRequest(Today);
            var transaction = Transaction.Create("Description 1", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Today);
            _repositoryMock.Setup(r => r.GetByIdAsync(transaction.Id)).ReturnsAsync(transaction);

            var result = await _service.PayAsync(transaction.Id, request);

            _repositoryMock.Verify(r => r.Update(transaction), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _dispatcherMock.Verify(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>()), Times.Once);
            Assert.IsType<TransactionResponse>(result);
            Assert.Equal(transaction.Id, result.Id);
            Assert.Equal(Today, result.PaymentDate);
            Assert.Equal(TransactionStatusDto.Paid.ToString(), result.Status);
        }

        [Fact]
        public async Task PayAsync_WhenTransactionIsCancelled_ShouldThrowTransactionPayException()
        {
            var request = new PayTransactionRequest(Today);
            var transaction = Transaction.Create("Description 1", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Today);
            transaction.Cancel();
            _repositoryMock.Setup(r => r.GetByIdAsync(transaction.Id)).ReturnsAsync(transaction);

            await Assert.ThrowsAsync<TransactionPayException>(() => _service.PayAsync(transaction.Id, request));
            _dispatcherMock.Verify(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>()), Times.Never);
            Assert.Equal(TransactionStatus.Cancelled, transaction.Status);
            Assert.Null(transaction.PaymentDate);
        }

        [Fact]
        public async Task PayAsync_WhenTransactionIsAlreadyPaid_ShouldThrowTransactionPayException()
        {
            var request = new PayTransactionRequest(Today);
            var transaction = Transaction.Create("Description 1", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Yesterday);
            transaction.Pay(Yesterday);
            _repositoryMock.Setup(r => r.GetByIdAsync(transaction.Id)).ReturnsAsync(transaction);

            await Assert.ThrowsAsync<TransactionPayException>(() => _service.PayAsync(transaction.Id, request));
            _dispatcherMock.Verify(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>()), Times.Never);
            Assert.Equal(TransactionStatus.Paid, transaction.Status);
            Assert.Equal(Yesterday, transaction.PaymentDate);
        }

        [Fact]
        public async Task PayAsync_WhenPaymentDateIsBeforeCreatedAt_ShouldThrowTransactionPayException()
        {
            var request = new PayTransactionRequest(Yesterday);
            var transaction = Transaction.Create("Description 1", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Today);
            _repositoryMock.Setup(r => r.GetByIdAsync(transaction.Id)).ReturnsAsync(transaction);

            await Assert.ThrowsAsync<TransactionPayException>(() => _service.PayAsync(transaction.Id, request));
            _dispatcherMock.Verify(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>()), Times.Never);
            Assert.Equal(TransactionStatus.Pending, transaction.Status);
            Assert.Null(transaction.PaymentDate);
        }
    }
}
