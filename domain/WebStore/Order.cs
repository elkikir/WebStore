namespace WebStore
{
    public class Order
    {
        public int Id { get; }

        private List<OrderItem> l_items;
        public IReadOnlyCollection<OrderItem> Items 
        {
            get { return l_items; } 
        }

        public int TotalCount
        {
            get { return l_items.Sum(item => item.Count); }
        }

        public Decimal TotalPrice
        {
            get { return l_items.Sum(item => (decimal)item.Price * item.Count); }
        }

        public Order(int id, IEnumerable<OrderItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            Id = id;
            l_items = new List<OrderItem>(items);
        }

        public void AddItem(Book book, int count) 
        { 
            if(book == null)
                throw new ArgumentNullException(nameof(book));
            var item = l_items.SingleOrDefault(x => x.BookId == book.Id);

            if (item == null)
                l_items.Add(new OrderItem(book.Id, book.Price, count));
            else
            {
                l_items.Remove(item);
                l_items.Add(new OrderItem(book.Id, item.Price, count + item.Count)); //test
            }

        }
    }
}
