using FinanceManager.Infrastructure.Exceptions;
using FinanceManager.Infrastructure.Repositories;
using FinanceManager.Infrastructure.Tests.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Infrastructure.Tests.Repositories.Category
{
    public class CategoryRepositoryTests(DatabaseFixture fixture)
        : IClassFixture<DatabaseFixture>
    {
        [Fact]
        public async Task AddAsync_ShouldAddCategory()
        {
            var category = FinanceManager.Domain.Entities.Category.Create("Category 1", "Description 1");
            var context = fixture.CreateContext();
            var sut = new CategoryRepository(context);

            await sut.AddAsync(category);
            var entry = context.Entry(category);

            Assert.NotNull(entry);
            Assert.Equal(EntityState.Added, entry.State);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCategoryExists_ShouldReturnCategory()
        {
            var category = FinanceManager.Domain.Entities.Category.Create("Category 1", "Description 1");
            var revenue = FinanceManager.Domain.Entities.Transaction.Create("Transaction 1", 100, DateTime.Now, Domain.Enums.TransactionType.Revenue, category.Id, DateTime.Now.AddDays(-1));
            var expense = FinanceManager.Domain.Entities.Transaction.Create("Transaction 1", 150, DateTime.Now, Domain.Enums.TransactionType.Expense, category.Id, DateTime.Now.AddDays(-1));
            var context = fixture.CreateContext();
            var sut = new CategoryRepository(context);
            
            await sut.AddAsync(category);
            await context.SaveChangesAsync();
            
            await context.Transactions.AddAsync(expense);
            await context.Transactions.AddAsync(revenue);
            await context.SaveChangesAsync();

            var categoryDb = await sut.GetByIdAsync(category.Id);

            Assert.NotNull(categoryDb);
            Assert.Equal(categoryDb.Name, category.Name);
            Assert.Equal(categoryDb.Description, category.Description);
            Assert.NotEmpty(categoryDb.Transactions);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCategoryDoesNotExist_ShouldThrowException()
        {
            var context = fixture.CreateContext();
            var sut = new CategoryRepository(context);
            var id = Guid.NewGuid();

            await Assert.ThrowsAsync<EntityNotFoundInfraException>(() => sut.GetByIdAsync(id));
        }
    }
}
