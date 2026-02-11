using FinanceManager.Domain.Repositories;
using FinanceManager.Infrastructure.Data;
using FinanceManager.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Infrastructure.Repositories
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        public async Task CommitAsync()
        {
            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private static void HandleException(Exception ex)
        {
            throw ex switch
            {
                _ => ex,
            };
        }
    }
}
