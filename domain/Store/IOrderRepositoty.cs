using System;
using System.Collections.Generic;
using System.Text;

namespace Store
{
    public interface IOrderRepositoty
    {
        OrderItem Create();

        OrderItem GetById(int id);

        void Update(OrderItem order);
    }
}
