namespace Finanza.Domain.Exceptions
{
    public class TransactionPayException : Exception
    {
        public TransactionPayException(string message) : base(message) { }
    }
}
