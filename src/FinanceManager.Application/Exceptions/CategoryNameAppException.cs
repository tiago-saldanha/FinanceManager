namespace Application.Exceptions
{
    public class CategoryNameAppException : Exception
    {
        public CategoryNameAppException() : base("O nome da categoria é obrigatório")
        {
        }
    }
}