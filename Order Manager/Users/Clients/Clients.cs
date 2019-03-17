using System;
using System.Collections.Generic;
using System.Text;

namespace Order_Manager.Users.Clients
{
    class Client : User, IClients
    {
        public void SendMessage(string fixMsg, string location)
        {
            throw new NotImplementedException();
        }

        public void begin()
        {
            throw new NotImplementedException();
        }
    }
}
