using Finanza.Domain.Repositories;
using Finanza.Infrastructure.Data;
using Finanza.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Finanza.Infrastructure.Repositories
{
    public class UnitOfWork(TenantDbContext context) : IUnitOfWork
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
