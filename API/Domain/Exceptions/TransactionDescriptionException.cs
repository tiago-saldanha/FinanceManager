namespace API.Domain.Exceptions
{
    public class TransactionDescriptionException : Exception
    {
        public TransactionDescriptionException() : base("A descrição deve ser informada") { }
    }
}
