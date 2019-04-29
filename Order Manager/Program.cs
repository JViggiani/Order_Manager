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
            OrderManagerThread.Name = "ORDERMANAGER_thread";
            OrderManagerThread.Start();

            Client client = new Client("CLIENT1");
            Thread ClientThread1 = new Thread(new ThreadStart(client.begin));
            ClientThread1.Name = "CLIENT1_thread";
            ClientThread1.Start();
        }
    }
}
