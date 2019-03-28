using System;
using System.Collections.Generic;
using System.Text;
using Order_Manager.FIX;

namespace Order_Manager.Orders
{
    // Concrete Creators
    public class OrderFactory : IOrderFactory
    {
        public Order getOrder(FixMessage fixMsg)
        {
            string rawFixMsg = fixMsg.getFixString();
            if (rawFixMsg.Contains("40=1")) { return new MarketOrder(); }  //placeholder if statements!
            else if (rawFixMsg.Contains("40=2")) { return new LimitOrder(); }
            else if (rawFixMsg.Contains("40=3")) { return new StopOrder(); }
            else
                throw new ArgumentException("Invalid FIX Message");
        }

        public FixMessage convertToFixMsg(string orderType, int orderQuantity, string side, string ticker)
        {
            FixMessage fixMessage = new FixMessage("");
            return fixMessage;
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
}
