using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Enums;
using Application.Exceptions;
using Domain.Entities;
using Moq;

namespace Application.Tests.Services.TransactionAppServiceTests
{
    public class CreateAsyncTests : TransactionAppServiceBaseTests
    {
        [Fact]
        public async Task CreateAsync_WhenRequestIsValid_ShouldCreateTransaction()
        {
            var request = new CreateTransactionRequest
            {
                Description = "Description 1",
                Amount = 100,
                DueDate = Tomorrow,
                TransactionType = "expense",
                CategoryId = Guid.Empty,
                CreatedAt = Today
            };

            var result = await _service.CreateAsync(request);
            
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            Assert.IsType<TransactionResponse>(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Null(result.PaymentDate);
            Assert.Equal(TransactionStatusDto.Pending.ToString(), result.Status);
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(request.Amount, result.Amount);
            Assert.Equal(request.DueDate, result.DueDate);
            Assert.Equal(request.CreatedAt, result.CreatedAt);
            Assert.Equal(request.TransactionType.ToString(), result.Type.ToLower());
        }

        [Theory]
        [InlineData("invalid_data")]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CreateAsync_WhenTransactionTypeIsInvalid_ShouldThrowTransactionTypeAppException(string transactionType)
        {
            var request = new CreateTransactionRequest
            {
                Description = "Description 1",
                Amount = 100,
                DueDate = Tomorrow,
                TransactionType = transactionType,
                CategoryId = Guid.Empty,
                CreatedAt = Today
            };

            await Assert.ThrowsAsync<TransactionTypeAppException>(() => _service.CreateAsync(request));
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Transaction>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
