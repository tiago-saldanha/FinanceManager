namespace Finanza.Domain.Exceptions
{
    public class TransactionUpdateException : Exception
    {
        public TransactionUpdateException(string message) : base(message)
        {
        }
    }
}
