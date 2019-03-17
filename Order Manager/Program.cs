using System;
using Order_Manager.Orders;
using Order_Manager.Users.Traders;

namespace Order_Manager
{
    class Program
    {
        static void Main(string[] args)
        {
            var orderFactory = new OrderFactory() as IOrderFactory;
            var marketOrder = orderFactory.GetOrder("a");

            Trader trader = new Trader();
            
        }
    }
}
