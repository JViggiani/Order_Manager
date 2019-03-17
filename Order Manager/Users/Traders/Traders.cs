using System;
using System.Collections.Generic;
using System.Text;

namespace Order_Manager.Users.Traders
{
    class Trader : User, ITraders
    {
        private List</*SOCKETS*/int> _clients;
        private string _id { get; set; }


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
