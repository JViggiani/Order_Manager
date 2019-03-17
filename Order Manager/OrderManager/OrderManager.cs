using System;
using System.Collections.Generic;
using System.Text;
using Order_Manager.Orders;

namespace Order_Manager.OrderManager
{
    class OrderManager : IOrderManager  //subject
    {
        public void begin()
        {
            throw new NotImplementedException();
        }

        public void finishTrading()
        {
            throw new NotImplementedException();
        }

        public void freezeTrading()
        {
            throw new NotImplementedException();
        }

        private void sendToExchange(Order order)
        {

        }
    }
}
