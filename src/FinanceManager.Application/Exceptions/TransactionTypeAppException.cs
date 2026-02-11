namespace Application.Exceptions
{
    public class TransactionTypeAppException : Exception
    {
        public TransactionTypeAppException() : base("Tipo da transação é inválido")
        {
        }
    }
}