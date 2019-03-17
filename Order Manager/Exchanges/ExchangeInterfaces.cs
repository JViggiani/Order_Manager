using System;
using System.Collections.Generic;
using System.Text;

namespace Order_Manager.Exchanges
{
    interface IExchange //subject
    {
        void routeOrder();
        void getPrice();
        void sendCancel();
        void openPort();
        void closePort();
    }
}
