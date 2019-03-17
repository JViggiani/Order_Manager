using System;
using System.Collections.Generic;
using System.Text;

namespace Order_Manager.Orders
{
    // Concrete Creators
    public class OrderFactory : IOrderFactory
    {
        public Order GetOrder(string rawFixMsg)
        {
            if (rawFixMsg.Contains("a")) { return new MarketOrder(); }  //placeholder if statements!
            else if (rawFixMsg.Contains("b")) { return new LimitOrder(); }
            else if (rawFixMsg.Contains("c")) { return new StopOrder(); }
            else if (rawFixMsg.Contains("d")) { return new BuyStopOrder(); }
            else
                throw new ArgumentException("Invalid FIX Message");
        }
    }

    //Product
    public abstract class Order
    {
        bool isBuy { get; set; }
        bool isSell { get; set; }
        int orderId { get; set; }
        int clientId { get; set; }
        int traderId { get; set; }
    }

    // Concrete Product
    public class MarketOrder : Order
    {

    }

    // Concrete Product
    public class LimitOrder : Order
    {

    }

    // Concrete Product
    public class StopOrder : Order
    {

    }

    // Concrete Product
    public class BuyStopOrder : Order
    {

    }
}
