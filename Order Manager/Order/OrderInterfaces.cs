using System;
using System.Collections.Generic;
using System.Text;
using Order_Manager.Orders;

namespace Order_Manager
{
    interface IOrderInterface
    {
        string rawFixMsg { get; set; }
        bool isBuy { get; set; }
        bool isSell { get; set; }
        int orderId { get; set; }
        int clientId{get;set;}
        int traderId { get; set; }


    }

    // Creator
    interface IOrderFactory
    {
        Order GetOrder(string rawFixMsg);
    }
}
