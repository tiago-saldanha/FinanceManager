using Finanza.Infrastructure.Exceptions;
using Finanza.Infrastructure.Repositories;
using Finanza.Infrastructure.Tests.Data;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Tests.Repositories.Category
{
    public class CategoryRepositoryTests(DatabaseFixture fixture)
        : IClassFixture<DatabaseFixture>
    {
        [Fact]
        public async Task AddAsync_ShouldAddCategory()
        {
            using var context = fixture.CreateContext();
            var sut = new CategoryRepository(context);
            
            var category = Domain.Entities.Category.Create("Category 1", "Description 1");

            await sut.AddAsync(category);
            var entry = context.Entry(category);

            Assert.Equal(EntityState.Added, entry.State);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCategoryExists_ShouldReturnCategory()
        {
            using var context = fixture.CreateContext();
            var sut = new CategoryRepository(context);
            
            var category = Domain.Entities.Category.Create("Category 1", "Description 1");
            
            var revenue = Domain.Entities.Transaction.Create("Transaction 1", 100, DateTime.Now, Domain.Enums.TransactionType.Revenue, category.Id, DateTime.Now.AddDays(-1));
            var expense = Domain.Entities.Transaction.Create("Transaction 1", 150, DateTime.Now, Domain.Enums.TransactionType.Expense, category.Id, DateTime.Now.AddDays(-1));
            
            await context.Categories.AddAsync(category);
            
            await context.Transactions.AddAsync(expense);
            await context.Transactions.AddAsync(revenue);
            
            await context.SaveChangesAsync();

            var result = await sut.GetByIdAsync(category.Id);

            Assert.Equal(result.Name, category.Name);
            Assert.Equal(result.Description, category.Description);
            Assert.NotEmpty(result.Transactions);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCategoryDoesNotExist_ShouldThrowException()
        {
            using var context = fixture.CreateContext();
            var sut = new CategoryRepository(context);

            await Assert.ThrowsAsync<EntityNotFoundInfraException>(() => sut.GetByIdAsync(Guid.Empty));
        }
    }
}
