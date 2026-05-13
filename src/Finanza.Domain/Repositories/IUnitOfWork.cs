namespace Finanza.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}
