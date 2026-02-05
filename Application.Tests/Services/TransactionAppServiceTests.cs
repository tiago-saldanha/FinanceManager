using System.Linq.Expressions;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Enums;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Repositories;
using Moq;

namespace Application.Tests.Services
{
    public class TransactionAppServiceTests
    {
        private readonly Mock<ITransactionRepository> _repositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly TransactionAppService _service;

        private static readonly DateTime Yesterday = new(2025, 12, 31);
        private static readonly DateTime Today = new(2026, 01, 01);
        private static readonly DateTime Tomorrow = new(2026, 01, 02);

        public TransactionAppServiceTests()
        {
            _repositoryMock = new Mock<ITransactionRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _service = new TransactionAppService(_repositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTransactionResponse()
        {
            var transaction = Transaction.Create("Description", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Today);

            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(transaction);
            var result = await _service.GetByIdAsync(transaction.Id);
            _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Once);

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

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTransactions()
        {
            var transactions = new List<Domain.Entities.Transaction>()
            {
                Transaction.Create("Description 1", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Today),
                Transaction.Create("Description 2", 200, Tomorrow, TransactionType.Expense, Guid.Empty, Today)
            };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(transactions);
            var result = await _service.GetAllAsync();
            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);

            var first = result.First();
            var last = result.Last();

            Assert.Equal(2, result.Count());
            Assert.Equal("Description 1", first.Description);
            Assert.Equal(100, first.Amount);
            Assert.Equal("Description 2", last.Description);
            Assert.Equal(200, last.Amount);
        }

        [Fact]
        public async Task GetByStatusAsync_ShouldReturnAllTransactionsByStatus()
        {
            var transactions = new List<Domain.Entities.Transaction>()
            {
                Transaction.Create("Description 1", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Today),
                Transaction.Create("Description 2", 200, Tomorrow, TransactionType.Expense, Guid.Empty, Today)
            };

            var status = TransactionStatusDto.pending;
            _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<Expression<Func<Transaction, bool>>>())).ReturnsAsync(transactions);
            var result = await _service.GetByStatusAsync(status);
            _repositoryMock.Verify(r => r.GetByFilterAsync(It.IsAny<Expression<Func<Transaction, bool>>>()), Times.Once);

            var first = result.First();
            var last = result.Last();
            Assert.Equal(2, result.Count());
            Assert.Equal("Description 1", first.Description);
            Assert.Equal(100, first.Amount);
            Assert.Equal("Description 2", last.Description);
            Assert.Equal(200, last.Amount);
        }

        [Fact]
        public async Task GetByTypeAsync_ShouldReturnAllTransactionsByType()
        {
            var transactions = new List<Domain.Entities.Transaction>()
            {
                Transaction.Create("Description 1", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Today),
                Transaction.Create("Description 2", 200, Tomorrow, TransactionType.Revenue, Guid.Empty, Today)
            };

            var type = TransactionTypeDto.revenue;
            _repositoryMock.Setup(r => r.GetByFilterAsync(It.IsAny<Expression<Func<Transaction, bool>>>())).ReturnsAsync(transactions);
            var result = await _service.GetByTypeAsync(type);
            _repositoryMock.Verify(r => r.GetByFilterAsync(It.IsAny<Expression<Func<Transaction, bool>>>()), Times.Once);

            var first = result.First();
            var last = result.Last();
            Assert.Equal(2, result.Count());
            Assert.Equal("Description 1", first.Description);
            Assert.Equal(type.ToString(), first.Type.ToLower());
            Assert.Equal(100, first.Amount);
            Assert.Equal("Description 2", last.Description);
            Assert.Equal(200, last.Amount);
            Assert.Equal(type.ToString(), last.Type.ToLower());
        }

        [Fact]
        public async Task PayAsync_ShouldReturnTheTransactionPaid()
        {
            var request = new PayTransactionRequest(Today);
            var transaction = Transaction.Create("Description 1", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Today);

            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(transaction);
            var result = await _service.PayAsync(It.IsAny<Guid>(), request);
            _repositoryMock.Verify(r => r.Update(transaction), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

            Assert.IsType<TransactionResponse>(result);
            Assert.Equal(transaction.Id, result.Id);
            Assert.Equal(Today, result.PaymentDate);
            Assert.Equal(TransactionStatusDto.paid.ToString(), result.Status.ToLower());
        }

        [Fact]
        public async Task ReopenAsync_ShouldReturnTheTransactionPending()
        {
            var transaction = Transaction.Create("Description 1", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Today);
            transaction.Pay(Today);

            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(transaction);
            var result = await _service.ReopenAsync(It.IsAny<Guid>());

            _repositoryMock.Verify(r => r.Update(transaction), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

            Assert.IsType<TransactionResponse>(result);
            Assert.Equal(transaction.Id, result.Id);
            Assert.Null(result.PaymentDate);
            Assert.Equal(TransactionStatusDto.pending.ToString(), result.Status.ToLower());
        }

        [Fact]
        public async Task CancelAsync_ShouldReturnTheTransactionCancelled()
        {
            var transaction = Transaction.Create("Description 1", 100, Tomorrow, TransactionType.Revenue, Guid.Empty, Today);

            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(transaction);
            var result = await _service.CancelAsync(It.IsAny<Guid>());

            _repositoryMock.Verify(r => r.Update(transaction), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

            Assert.IsType<TransactionResponse>(result);
            Assert.Equal(transaction.Id, result.Id);
            Assert.Null(result.PaymentDate);
            Assert.Equal(TransactionStatusDto.cancelled.ToString(), result.Status.ToLower());
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnTheNewTransaction()
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
            Assert.Equal(TransactionStatusDto.pending.ToString(), result.Status.ToLower());
            Assert.Equal(request.Description, result.Description);
            Assert.Equal(request.Amount, result.Amount);
            Assert.Equal(request.DueDate, result.DueDate);
            Assert.Equal(request.CreatedAt, result.CreatedAt);
            Assert.Equal(request.TransactionType.ToString(), result.Type.ToLower());
        }
    }
}
