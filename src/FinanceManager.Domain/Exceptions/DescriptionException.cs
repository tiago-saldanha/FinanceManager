namespace Domain.Exceptions
{
    public class DescriptionException : Exception
    {
        public DescriptionException() : base("A descrição deve ser informada") { }
    }
}
