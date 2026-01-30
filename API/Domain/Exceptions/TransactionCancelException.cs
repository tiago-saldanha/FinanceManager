namespace API.Domain.Exceptions
{
    public class TransactionCancelException : Exception
    {
        public TransactionCancelException() : base("Não é possível cancelar uma transação que já foi paga")
        {
        }
    }
}