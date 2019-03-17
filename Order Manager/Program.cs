using System;
using Order_Manager.Orders;

namespace Order_Manager
{
    class Program
    {
        static void Main(string[] args)
        {
            var orderFactory = new OrderFactory() as IOrderFactory;
            var marketOrder = orderFactory.GetOrder("a");
        }
    }
}
