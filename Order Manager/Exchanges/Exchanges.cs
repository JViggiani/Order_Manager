using System;
using System.Collections.Generic;
using System.Text;

namespace Order_Manager.Exchanges
{
    public abstract class Exchange : IExchange
    {
        string name;

        public void closePort()
        {
            throw new NotImplementedException();
        }

        public void getPrice()
        {
            throw new NotImplementedException();
        }

        public void openPort()
        {
            throw new NotImplementedException();
        }

        public void routeOrder()
        {
            throw new NotImplementedException();
        }

        public void sendCancel()
        {
            throw new NotImplementedException();
        }
    }
}
