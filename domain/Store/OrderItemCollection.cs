using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Store
{
    public class OrderItemCollection : IReadOnlyCollection<OrderItem>
    {
        readonly List<OrderItem> items;

        public OrderItemCollection(IEnumerable<OrderItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            this.items = new List<OrderItem>(items);
        }

        public int Count => items.Count;

        public OrderItem Get(int bookId)
        {
            if (TryGet(bookId, out OrderItem orderItem))
                return orderItem;
            throw new InvalidOperationException("Book not Found");
        }

        public bool TryGet(int bookId, out OrderItem orderItem)
        {
            var index = items.FindIndex(i => i.BookId == bookId);
            if(index == -1)
            {
                orderItem = null;
                return false;
            }
            orderItem = items[index];
            return true;
        }



        public IEnumerator<OrderItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (items as IEnumerable).GetEnumerator();
        }
    }
}
