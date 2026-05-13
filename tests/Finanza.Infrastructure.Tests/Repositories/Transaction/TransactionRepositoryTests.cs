using System.Linq.Expressions;
using Finanza.Infrastructure.Exceptions;
using Finanza.Infrastructure.Repositories;
using Finanza.Infrastructure.Tests.Data;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Tests.Repositories.Transaction
{
    public class TransactionRepositoryTests(DatabaseFixture fixture)
        : IClassFixture<DatabaseFixture>
    {
        private static readonly DateTime Today = new(2026, 01, 01);
        private static readonly DateTime Yesterday = new(2025, 12, 31);

        [Fact]
        public async Task AddAsync_ShouldAddTransaction()
        {
            using var context = fixture.CreateContext();
            var sut = new TransactionRepository(context);
            
            var category = GetCategory();
            var transaction = GetTransaction(Domain.Enums.TransactionType.Revenue, category.Id);
            
            context.Categories.Add(category);
            
            await sut.AddAsync(transaction);
            
            var entry = context.Entry(transaction);

            Assert.Equal(EntityState.Added, entry.State);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTransactions()
        {
            using var context = fixture.CreateContext();
            var sut = new TransactionRepository(context);
            
            var category = GetCategory();
            context.Categories.Add(category);
            
            context.Transactions.AddRange(GetTransaction(Domain.Enums.TransactionType.Revenue, category.Id), GetTransaction(Domain.Enums.TransactionType.Expense, category.Id));
            
            await context.SaveChangesAsync();

            var result = await sut.GetAllAsync();

            Assert.Equal(2, result.Count);
            Assert.All(result, x => Assert.NotNull(x.Category));
        }

        [Fact]
        public async Task GetByIdAsync_WhenTransactionExists_ShouldReturnTransaction()
        {
            using var context = fixture.CreateContext();
            var sut = new TransactionRepository(context);
            
            var category = GetCategory();
            var transaction = GetTransaction(Domain.Enums.TransactionType.Revenue, category.Id);
            
            context.Categories.Add(category);
            context.Transactions.Add(transaction);
            
            await context.SaveChangesAsync();

            var result = await sut.GetByIdAsync(transaction.Id);

            Assert.Equal(transaction.Id, result.Id);
            Assert.Equal(category.Id, result.Category.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenTransactionDoesNotExists_ShouldThrowException()
        {
            using var context = fixture.CreateContext();
            var sut = new TransactionRepository(context);

            await Assert.ThrowsAsync<EntityNotFoundInfraException>(() => sut.GetByIdAsync(Guid.Empty));
        }

        [Fact]
        public async Task Update_ShouldUpdateTransaction()
        {
            using var context = fixture.CreateContext();
            var sut = new TransactionRepository(context);

            var category = GetCategory();
            var transaction = GetTransaction(Domain.Enums.TransactionType.Revenue, category.Id);

            context.Categories.Add(category);
            context.Transactions.Add(transaction);

            await context.SaveChangesAsync();

            transaction.Pay(Today);

            sut.Update(transaction);

            Assert.Equal(EntityState.Modified, context.Entry(transaction).State);
        }

        [Fact]
        public async Task GetByFilterAsync_ShouldReturnOnlyFilteredTransactions()
        {
            using var context = fixture.CreateContext();
            var sut = new TransactionRepository(context);

            var category = GetCategory();

            var revenue = GetTransaction(Domain.Enums.TransactionType.Revenue, category.Id);
            var expense = GetTransaction(Domain.Enums.TransactionType.Revenue, category.Id);

            revenue.Pay(Today);
            expense.Cancel();

            context.Categories.Add(category);
            context.Transactions.AddRange(revenue, expense);

            await context.SaveChangesAsync();

            Expression<Func<Domain.Entities.Transaction, bool>> filter =
                x => x.Status == revenue.Status;

            var result = await sut.GetByFilterAsync(filter);

            Assert.Single(result);
            Assert.Equal(revenue.Id, result.First().Id);
        }

        private static Domain.Entities.Category GetCategory()
            => Domain.Entities.Category.Create("Category 1", "Description 1");

        private static Domain.Entities.Transaction GetTransaction(Domain.Enums.TransactionType type, Guid categoryId)
            => Domain.Entities.Transaction.Create("Transaction 1", 100, Today, type, categoryId, Yesterday);
    }
}
