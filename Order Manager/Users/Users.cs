using System;
using System.Collections.Generic;
using System.Text;

namespace Order_Manager.Users
{
    public abstract class User : IUsers
    {
        private string Id { get; set; }

        public void begin()
        {
            throw new NotImplementedException();
        }

        public void SendMessage(string fixMsg, string location)
        {
            throw new NotImplementedException();
        }
    }
}
