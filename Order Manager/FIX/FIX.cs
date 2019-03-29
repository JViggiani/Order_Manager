using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Order_Manager.FIX
{
    public class FixMessage : IFixMessage
    {
        public FixMessage(string fixString)
        {
            this.setFixString(fixString);
        }

        private IDictionary<int, string> _fixDict;

        public string getFixString()
        {
            string versionRaw = "";
            string byteLengthRaw = "";
            string msgTypeRaw = "";
            string senderCompIdRaw = "";
            string targetCompIdRaw = "";
            string msgSeqNumRaw = "";
            string sendingTimeRaw = "";
            string checksumRaw = "";

            if (_fixDict.ContainsKey(8))
            {
                versionRaw = _fixDict[8];
            }
            if (_fixDict.ContainsKey(9))
            {
                byteLengthRaw = _fixDict[9];
            }
            if (_fixDict.ContainsKey(35))
            {
                msgTypeRaw = _fixDict[35];
            }
            if (_fixDict.ContainsKey(49))
            {
                senderCompIdRaw = _fixDict[49];
            }
            if (_fixDict.ContainsKey(56))
            {
                targetCompIdRaw = _fixDict[56];
            }
            if (_fixDict.ContainsKey(34))
            {
                msgSeqNumRaw = _fixDict[34];
            }
            if (_fixDict.ContainsKey(52))
            {
                sendingTimeRaw = _fixDict[52];
            }
            if (_fixDict.ContainsKey(10))
            {
                checksumRaw = _fixDict[10];
            }

            string version = "8=" + versionRaw + "|";
            string byteLength = "";
            if (_fixDict.ContainsKey(9))
            {
                byteLength = "9=" + byteLengthRaw + "|";
            }
            string msgType = "35=" + msgTypeRaw + "|";
            string senderCompId = "49=" + senderCompIdRaw + "|";
            string targetCompId = "56=" + targetCompIdRaw + "|";
            string msgSeqNum = "34=" + msgSeqNumRaw + "|";
            string sendingTime = "52=" + sendingTimeRaw + "|";
            string checksum = "|10=" + checksumRaw;

            _fixDict.Remove(8);
            _fixDict.Remove(9);
            _fixDict.Remove(35);
            _fixDict.Remove(49);
            _fixDict.Remove(56);
            _fixDict.Remove(34);
            _fixDict.Remove(52);
            _fixDict.Remove(10);

            string bulk = string.Join("|", _fixDict.Select(x => x.Key + "=" + x.Value));

            _fixDict[8] = versionRaw;
            _fixDict[9] = byteLengthRaw;
            _fixDict[35] = msgTypeRaw;
            _fixDict[49] = senderCompIdRaw;
            _fixDict[56] = targetCompIdRaw;
            _fixDict[34] = msgSeqNumRaw;
            _fixDict[52] = sendingTimeRaw;
            _fixDict[10] = checksumRaw;

            string fixString = version + byteLength + msgType + senderCompId + 
                targetCompId + msgSeqNum + sendingTime + bulk + checksum;
            return fixString;
        }

        public void setFixString(string fixString)
        {
            _fixDict = fixString.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(part => part.Split('='))
               .ToDictionary(split => int.Parse(split[0]), split => split[1]);
            //put 35 at the front
            this.wrapWithMetaData();
        }

        public void addToFixMsg(string fixAppend)
        {
            string[] elementArray = fixAppend.Split("|");
            foreach(string element in elementArray)
            {
                string[] individualElement = element.Split("=");
                _fixDict[int.Parse(individualElement[0])] = individualElement[1];
            }
            this.wrapWithMetaData();
        }

        private void wrapWithMetaData()
        {
            _fixDict[8] = "FIX.4.4";
            _fixDict[10] = "CHK";   //TODO placeholder checksum
            _fixDict[9] = (getFixString().Length + getFixString().Length.ToString().Length + 3).ToString();
        }

        public int getHeartBeatInterval()
        {
            return int.Parse(_fixDict[108]);
        }
        public void setHeartBeatInterval(float interval)
        {
            this._fixDict[108] = interval.ToString();
        }

        public void setSenderId(string userId)
        {
            addToFixMsg("49=" + userId);
        }
        public void setTargetId(string targetId)
        {
            addToFixMsg("56=" + targetId);
        }

        public void setMsgSeqNum(int seqNum)
        {
            addToFixMsg("34=" + seqNum.ToString());
        }

        public void setSendingTime(string sendingTime)
        {
            addToFixMsg("52=" + sendingTime);
        }
    }
}
