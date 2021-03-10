using System;
using System.Collections.Generic;
using System.Linq;

namespace Store
{
    public class OrderItem
    {
        public int Id { get; }

        private List<OrderItem> items;

        public IReadOnlyCollection<OrderItem> Items => items;

        public string CellPhone { get; set; }

        public OrderDelivery Delivery { get; set; }

        public OrderPayment Payment { get; set; }

        public int TotalCount => items.Sum(item => item.Count);

        public decimal TotalPrice => items.Sum(item => item.Price * item.Count) + (Delivery?.Amount ?? 0m); 

        public OrderItem(int id,IEnumerable<OrderItem> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            Id = id;
            this.items = new List<OrderItem>(items);
        }

        public OrderItem Get(int bookId)
        {
            var index = items.FindIndex(i => i.BookId == bookId);

            if (index == -1)
                ThrowBookException("Book not found", bookId);

            return items[index];
        }

        public void AddOrUpdateItem(Book book, int count)
        {
            if (book == null)
                throw new ArgumentNullException(nameof(book));

            var index = items.FindIndex(i => i.BookId == book.Id);
            if (index == -1)
                items.Add(new OrderItem(book.Id, book.Price, count));
            else
                items[index].Count += count;
        }

        public void RemoveItem(int bookId)
        {

            var index = items.FindIndex(i => i.BookId == bookId);

            if (index == -1)
                ThrowBookException("Order does not contain item with item.", bookId);

            items.RemoveAt(index);
        }

        void ThrowBookException(string mes, int bookId)
        {
            var exc = new InvalidOperationException(mes);
            exc.Data["BookId"] = bookId;

            throw exc;
        }
    }
}
