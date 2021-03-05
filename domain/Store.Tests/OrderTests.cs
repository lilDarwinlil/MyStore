using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Store.Tests
{
    public class OrderTests
    {
        [Fact]
        public void Order_WithNullItems_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Order(1,null));
        }

        [Fact]
        public void TotalCount_WithEmptyItems_ReturnZero()
        {
            Assert.Equal(0m, (new Order(1, new OrderItem[0])).TotalCount);
        }

        [Fact]
        public void TotalPrice_WithItems_Calculates()
        {
            Assert.Equal(1050m, (new Order(1, new []{ new OrderItem(1, 5, 10m) , new OrderItem (2,10,100m)})).TotalPrice);
        }
    }
}
