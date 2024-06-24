namespace WebStore
{
    public class OrderItem
    {
        public int BookId { get; }
        public decimal Price { get; }

        private int count;
        public int Count
        {
            get => count;
            set => count = value > 0 ? value : throw new ArgumentOutOfRangeException("Count must be greater then null");
        }

        public OrderItem (int bookId, decimal price, int count)
        {
            BookId = bookId;
            Price = price;
            Count = count;
        }

    }
}
