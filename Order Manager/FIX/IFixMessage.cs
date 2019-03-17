using System;
using System.Collections.Generic;
using System.Text;

namespace Order_Manager.FIX
{
    interface IFixMessage
    {
        string parseToString();
    }
}
