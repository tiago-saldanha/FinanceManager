namespace Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}
