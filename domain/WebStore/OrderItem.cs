using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStore
{
    public class OrderItem
    {
        public int BookId { get; }
        public decimal Price { get; }
        public int Count { get; }

        public OrderItem (int bookId, decimal price, int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("Count must be greater then null");
            BookId = bookId;
            Price = price;
            Count = count;
        }

    }
}
