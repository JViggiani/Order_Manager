using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Order_Manager.Users
{
    public interface IUsers //Subscriber in pub-sub
    {
        void begin();
    }
}
