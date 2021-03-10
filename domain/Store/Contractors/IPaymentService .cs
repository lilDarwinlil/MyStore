using System.Collections.Generic;

namespace Store.Contractors
{
    public interface IPaymentService
    {
        string UniqueCode { get; }

        string Title { get; }

        Form CreateForm(OrderItem order);

        Form MoveNextForm(int orderId, int step, IReadOnlyDictionary<string,string> value);

        OrderPayment GetPayment(Form form);
    }
}
