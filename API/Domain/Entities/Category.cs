namespace API.Domain.Entities
{
    public class Category
    {
        public Category()
        {
        }

        public Category(Guid id, string name, string? description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }

        public virtual ICollection<Transaction> Transactions { get; set; } = [];
    }
}