using System;
using System.Collections.Generic;
using System.Text;

namespace Order_Manager.FIX
{
    interface IFixMessage
    {
        string getFixString();
        int getHeartBeatInterval();
        void setSenderId(string userId);
        void setTargetId(string targetId);
        void setMsgSeqNum(int seqNum);
        void setSendingTime(string sendingTime);
    }
}
