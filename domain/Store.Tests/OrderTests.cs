using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Store.Tests
{
    public class OrderTests
    {
        OrderItem GenerateOrders => new OrderItem(1, new[] { new OrderItem(1, 10m, 5), new OrderItem(2, 100m, 10) });

        [Fact]
        public void Order_WithNullItems_ThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new OrderItem(1, null));
        }

        [Fact]
        public void TotalCount_WithEmptyItems_ReturnZero()
        {
            Assert.Equal(0m, (new OrderItem(1, new OrderItem[0])).TotalCount);
        }

        [Fact]
        public void TotalPrice_WithItems_Calculates()
        {
            Assert.Equal(1050m, GenerateOrders.TotalPrice);
        }

        [Fact]
        public void Get_WithExistingItem_ReturnsItem()
        {
            Assert.Equal(5, GenerateOrders.Get(1).Count);
        }

        [Fact]
        public void Get_WithNonItem_ThrowsInvalidOperation()
        {
            Assert.Throws<InvalidOperationException>(() => GenerateOrders.Get(3));
        }

        [Fact]
        public void AddOrUpdateItem_WithExistingItem_UpdateCount()
        {
            var o = GenerateOrders;
            o.AddOrUpdateItem(new Book(1, "", "", "", "", 0m), 10);
            Assert.Equal(15, o.Get(1).Count );
        }

        [Fact]
        public void AddOrUpdateItem_WithNoneItem_AddsCount()
        {
            var o = GenerateOrders;
            o.AddOrUpdateItem(new Book(3, "", "", "", "", 0m), 10);
            Assert.Equal(5,  o.Get(1).Count);
            Assert.Equal(10, o.Get(3).Count);
        }


        [Fact]
        public void RemoveItem_WithExistingItem_RemovesItem()
        {
            var o = GenerateOrders; 
            o.RemoveItem(1);
            Assert.Equal(1, o.Items.Count);
        }

        [Fact]
        public void RemoveItem_WithNonItem_ThrowsInvalidOperation()
        {
            Assert.Throws<InvalidOperationException>(() => GenerateOrders.RemoveItem(3));
        }
    }
}
