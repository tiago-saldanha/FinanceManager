
namespace Application.Services
{
    public class InvalidCategoryNameException : Exception
    {
        public InvalidCategoryNameException() : base("O nome da categoria é obrigatório")
        {
        }
    }
}