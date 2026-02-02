namespace Infrastructure.Exceptions
{
    public class EntityNotFoundInfraException : Exception
    {
        public EntityNotFoundInfraException(string message) : base(message)
        {
        }
    }
}