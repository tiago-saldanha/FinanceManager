namespace FinanceManager.Infrastructure.Exceptions
{
    public class EntityAlreadyExistsInfraException : Exception
    {
        public EntityAlreadyExistsInfraException() : base("Já existe uma categoria cadastrada com este nome") { }
    }
}
