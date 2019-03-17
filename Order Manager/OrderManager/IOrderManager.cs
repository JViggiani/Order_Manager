using System;
using System.Collections.Generic;
using System.Text;
using Order_Manager.Exchanges;

namespace Order_Manager.OrderManager
{
    //todo implement entity framework and save trades, addresses etc to a database
    interface IOrderManager //Message broker in Pub-Sub
    {
        void begin();
        void finishTrading();
        void freezeTrading();
        void update(IExchange exchange);
    }
}
