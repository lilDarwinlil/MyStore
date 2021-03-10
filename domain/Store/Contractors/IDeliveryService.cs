using System.Collections.Generic;

namespace Store.Contractors
{
    public interface IDeliveryService
    {
        string UniqueCode { get; }

        string Title { get; }

        Form CreateForm(OrderItem order);

        Form MoveNextForm(int orderId, int step, IReadOnlyDictionary<string,string> value);

        OrderDelivery GetDelivery(Form form);
    }
}
