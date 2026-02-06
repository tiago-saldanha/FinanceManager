using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Category
    {
        protected Category() { }

        private Category(Guid id, Description name, string? description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public static Category Create(string name, string? description)
        {
            return new Category(
                Guid.NewGuid(),
                new Description(name),
                description
            );
        }

        public Guid Id { get; private set; }
        public Description Name { get; private set; }
        public string? Description { get; private set; }
        public virtual ICollection<Transaction> Transactions { get; set; } = [];
    }
}