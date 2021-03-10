using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Store.Memory
{
    public class OrderRepository : IOrderRepositoty
    {
        readonly List<OrderItem> orders = new List<OrderItem>();

        public OrderItem Create()
        {
            int nextId = orders.Count + 1;
            var order = new OrderItem(nextId, new OrderItem[0]);

            orders.Add(order);

            return order;
        }

        public OrderItem GetById(int id)
        {
            return orders.Single(o => o.Id == id);
        }

        public void Update(OrderItem order)
        {
            //orders[order.Id] = order;
        }
    }
}
