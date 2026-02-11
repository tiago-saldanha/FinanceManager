namespace Application.Exceptions
{
    public class TransactionStatusAppException : Exception
    {
        public TransactionStatusAppException() : base("Status da transação é inválido")
        {
        }
    }
}