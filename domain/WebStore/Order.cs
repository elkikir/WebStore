﻿namespace WebStore
{
    public class Order
    {
        public int Id { get; }

        private List<OrderItem> items;
        public IReadOnlyCollection<OrderItem> Items => items;

        public string CellPhone { get; set; }

        public OrderDelivery Delivery { get; set; }

        public OrderPayment Payment { get; set; }

        public int TotalCount => items.Sum(item => item.Count);

        public Decimal TotalPrice => items.Sum(item => (decimal)item.Price * item.Count) 
                                     + (Delivery?.Amount ?? 0m);

        public Order(int id, IEnumerable<OrderItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            Id = id;
            this.items = new List<OrderItem>(items);
        }

        public OrderItem GetItem(int bookId)
        {
            return items.Single(item => item.BookId == bookId);
        }

        public void AddItem(Book book, int count = 1)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));
            var index = items.FindIndex(item => item.BookId == book.Id);

            if (index == -1)
                items.Add(new OrderItem(book.Id, book.Price, count: 1));
            else
            {
                items[index].Count += count;
            }
        }

        public void RemoveItem(Book book, int count)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));
            var index = items.FindIndex(item => item.BookId == book.Id);

            if (index == -1)
                throw (new ArgumentException("Book must be exist in order", nameof(book)));
            else if (items[index].Count + count <= 0)
                DeleteItem(book);
            else
                items[index].Count += count;
        }

        public void DeleteItem(Book book)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            var item = items.SingleOrDefault(x => x.BookId == book.Id);
            if (!items.Remove(item))
                throw (new ArgumentException("Book must be exist in order", nameof(book)));
        }
    }
}
