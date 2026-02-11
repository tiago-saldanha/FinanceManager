namespace Domain.Exceptions
{
    public class TransactionReopenException : Exception
    {
        public TransactionReopenException() : base("A transação não está paga")
        {
        }
    }
}