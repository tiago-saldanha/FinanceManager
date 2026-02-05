using Application.Services;
using Domain.Repositories;
using Moq;

namespace Application.Tests.Services.CategoryAppServiceTests
{
    public class CategoryAppServiceBaseTests
    {
        protected readonly Mock<ICategoryRepository> _repositoryMock;
        protected readonly Mock<IUnitOfWork> _unitOfWork;
        protected readonly CategoryAppService _service;

        public CategoryAppServiceBaseTests()
        {
            _repositoryMock = new Mock<ICategoryRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _service = new CategoryAppService(_repositoryMock.Object, _unitOfWork.Object);
        }
    }
}
