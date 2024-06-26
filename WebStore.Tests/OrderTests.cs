﻿using NPOI.SS.Formula.Functions;

namespace WebStore.Tests
{
    public class OrderTests
    {
        [Fact]
        public void Order_WithNullItems_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Order(0, items: null));
        }

        [Fact]
        public void TotalCount_WithEmptyItems_ReturnsZero()
        {
            var order = new Order(1, Array.Empty<OrderItem>());
            Assert.Equal(0, order.TotalCount);
        }

        [Fact]
        public void TotalPrice_WithEmptyItems_ReturnsZero()
        {
            var order = new Order(1, Array.Empty<OrderItem>());
            Assert.Equal(0m, order.TotalPrice);
        }

        [Fact]
        public void TotalCount_WithNonEmptyItems_CalculateTotalCount()
        {
            var order = new Order(1, new OrderItem[4] 
            {
                new OrderItem(0, 0m, 2),
                new OrderItem(0, 0m, 3),
                new OrderItem(0, 0m, 1),
                new OrderItem(0, 0m, 1)
            });

            Assert.Equal(2 + 3 + 1 + 1, order.TotalCount);
        }

        [Fact]
        public void TotalPrice_WithNonEmptyItems_CalculateTotalCount()
        {
            var order = new Order(1, new OrderItem[4]
            {
                new OrderItem(0, 1m, 1),
                new OrderItem(0, 100m, 2),
                new OrderItem(0, 9m, 1),
                new OrderItem(0, 15m, 3)
            });

            Assert.Equal(1m + 100m * 2 + 9m + 15m * 3, order.TotalPrice);
        }

        [Fact]
        public void AddItem_WithBookEqualNull_ReturnsNull()
        {
            var order = new Order(0, new OrderItem[0]);
            Assert.Throws<ArgumentNullException>(() => order.AddItem(book: null));
        }

        [Fact]
        public void AddItem_WithBookExist_ReturnSumOfCount()
        {
            var order = new Order(0, new OrderItem[1] { new OrderItem(1, 1, 1) });
            order.AddItem(new Book(1, "", "", "", "", 1m));
            Assert.Equal(2, order.TotalCount);
        }

        [Fact]
        public void DeleteItem_WithBookEqualToNull_ThrowsException()
        {
            var order = new Order(0, new OrderItem[0]);
            Assert.Throws<ArgumentNullException>(() => order.DeleteItem(book: null));
        }

        [Fact]
        public void DeleteItem_WithBookDoesNotExistInOrder_ThrowsException()
        {
            var order = new Order(0, new OrderItem[1] { new OrderItem(1, 0, 1) });
            Assert.Throws<ArgumentException>(() => order.DeleteItem(new Book(2, "", "", "", "", 0m)));
        }

        [Fact]
        public void GetItem_WithItemDoesNotExist_ThrowsInvalidOperationException()
        {
            var order = new Order(0, new OrderItem[1] { new OrderItem(2, 0m, 1)});
            Assert.Throws<InvalidOperationException>(() => order.GetItem(1));
        }
    }
}
