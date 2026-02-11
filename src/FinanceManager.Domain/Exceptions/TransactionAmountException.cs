namespace Domain.Exceptions
{
    public class TransactionAmountException : Exception
    {
        public TransactionAmountException() : base("O valor não pode ser negativo")
        {
        }
    }
}