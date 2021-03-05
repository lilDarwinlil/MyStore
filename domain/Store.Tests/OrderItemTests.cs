using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Store.Tests
{
    public class OrderItemTests
    {
        [Fact]
        public void OrderItem_WithZeroCount_ThrowArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new OrderItem(1,0,0m));
        }

        [Fact]
        public void OrderItem_WithNegativeCount_ThrowArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new OrderItem(1, -1, 0m));
        }

        [Fact]
        public void OrderItem_WithPozitiveCount_SetsCount()
        {
            var orderItem = new OrderItem(1, 2, 3m);

            Assert.Equal(1,  orderItem.BookId);
            Assert.Equal(2,  orderItem.Count);
            Assert.Equal(3m, orderItem.Price);
        }
    }
}
