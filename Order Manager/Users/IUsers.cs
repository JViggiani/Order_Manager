using System;
using System.Collections.Generic;
using System.Text;

namespace Order_Manager.Users
{
    public interface IUsers //Subscriber in pub-sub
    {
        void SendMessage(string fixMsg, string location);   //location is a placeholder type. Needs to be some kind of ip / port object. Socket? todo
        void begin();
    }
}
