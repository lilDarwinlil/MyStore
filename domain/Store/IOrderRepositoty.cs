using System;
using System.Collections.Generic;
using System.Text;

namespace Store
{
    public interface IOrderRepositoty
    {
        Order Create();

        Order GetById(int id);

        void Update(Order order);
    }
}
