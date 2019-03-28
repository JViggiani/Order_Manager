using System;
using Order_Manager.Orders;
using Order_Manager.Users.Traders;
using Order_Manager.Order_Manager;
using System.Threading;
using Order_Manager.Users.Clients;

namespace Order_Manager
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            var orderFactory = new OrderFactory() as IOrderFactory;
            var marketOrder = orderFactory.GetOrder("a");

            Trader trader = new Trader();
            */

            OrderManager orderManager = new OrderManager();
            Thread OrderManagerThread = new Thread(new ThreadStart(orderManager.begin));
            OrderManagerThread.Start();

            Client client = new Client();
            Thread ClientThread = new Thread(new ThreadStart(client.begin));
            ClientThread.Start();
        }
    }
}
