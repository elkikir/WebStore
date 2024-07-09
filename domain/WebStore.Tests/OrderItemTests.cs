namespace WebStore.Tests
{
    public class OrderItemTests
    {
        [Fact]
        public void OrderItem_WithZeroCount_throwArgumentException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new OrderItem(0, 0.0m, count: 0));
        }

        [Fact]
        public void OrderItem_WithPositiveCount()
        {
            var order = new OrderItem(1, 2m, 3);
            Assert.Equal(1, order.BookId);
            Assert.Equal(2, order.Price);
            Assert.Equal(3, order.Count);
        }
    }
}
