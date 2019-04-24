using System;
using System.Collections.Generic;
using System.Text;
using Order_Manager.Orders;
using Order_Manager.FIX;

namespace Order_Manager
{
    interface IOrderInterface
    {
        
    }

    // Creator
    interface IOrderFactory
    {
        Order getOrder(FixMessage rawFixMsg);
        FixMessage convertToFixMsg(string orderType, int orderQuantity, string side, string ticker);
    }
}
