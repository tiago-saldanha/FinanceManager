using Application.Dispatchers;
using Application.Interfaces.Dispatchers;
using Application.Services;
using Domain.Repositories;
using Moq;

namespace Application.Tests.Services.TransactionAppServiceTests
{
    public class TransactionAppServiceBaseTests
    {
        protected readonly Mock<ITransactionRepository> _repositoryMock;
        protected readonly Mock<IUnitOfWork> _unitOfWorkMock;
        protected readonly Mock<IDomainEventDispatcher> _dispatcherMock;
        protected readonly TransactionAppService _service;

        protected readonly DateTime Yesterday = new(2025, 12, 31);
        protected readonly DateTime Today = new(2026, 01, 01);
        protected readonly DateTime Tomorrow = new(2026, 01, 02);

        public TransactionAppServiceBaseTests()
        {
            _repositoryMock = new Mock<ITransactionRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _dispatcherMock = new Mock<IDomainEventDispatcher>();
            _service = new TransactionAppService(_repositoryMock.Object, _unitOfWorkMock.Object, _dispatcherMock.Object);
        }        
    }
}
