namespace Domain.Entities
{
    public class Category
    {
        protected Category()
        {
        }

        private Category(Guid id, string name, string? description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public static Category Create(string name, string? description)
        {
            return new Category(
                Guid.NewGuid(),
                name,
                description
            );
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }

        public virtual ICollection<Transaction> Transactions { get; set; } = [];
    }
}