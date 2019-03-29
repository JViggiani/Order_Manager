using Order_Manager.FIX;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Order_Manager.Users.Clients
{
    class Client : User, IClients
    {
        public Client(string userId)
        {
            this.userId = userId;
        }

        public void begin()
        {
            Thread socketHandler = new Thread(new ThreadStart(StartClient));
            socketHandler.Start();
        }
    }
}
