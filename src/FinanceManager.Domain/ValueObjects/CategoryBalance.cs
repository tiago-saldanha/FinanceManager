namespace Domain.ValueObjects
{
    public readonly record struct CategoryBalance
    {
        public CategoryBalance(decimal received, decimal spent, decimal balance)
        {
            Received = received;
            Spent = spent;
            Balance = balance;
        }

        public decimal Received { get; }
        public decimal Spent { get; }
        public decimal Balance { get; }
    }
}
