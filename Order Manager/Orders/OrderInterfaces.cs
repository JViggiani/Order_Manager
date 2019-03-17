using System;
using System.Collections.Generic;
using System.Text;
using Order_Manager.Orders;
using Order_Manager.FIX;

namespace Order_Manager
{
    interface IOrderInterface
    {
        string convertToFixMsg();
    }

    // Creator
    interface IOrderFactory
    {
        Order GetOrder(string rawFixMsg);
    }
}
