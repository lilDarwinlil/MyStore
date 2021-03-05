using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Store.Memory
{
    public class OrderRepository : IOrderRepositoty
    {
        readonly List<Order> orders = new List<Order>();

        public Order Create()
        {
            int nextId = orders.Count + 1;
            var order = new Order(nextId, new OrderItem[0]);

            orders.Add(order);

            return order;
        }

        public Order GetById(int id)
        {
            return orders.Single(o => o.Id == id);
        }

        public void Update(Order order)
        {
            //orders[order.Id] = order;
        }
    }
}
