namespace Domain.Exceptions
{
    public class TransactionDateException : Exception
    {
        public TransactionDateException() : base("A data de vencimento não pode ser anterior à data de criação")
        {
        }
    }
}