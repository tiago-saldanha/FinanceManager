namespace Domain.Exceptions
{
    public class TransactionCancelException : Exception
    {
        public TransactionCancelException(string message) : base(message)
        {
        }
    }
}