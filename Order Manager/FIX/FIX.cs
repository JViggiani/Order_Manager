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

        //Objective: get the fix msg as a string. 8 must come first. 9 must come second. 35 must come third. 10 must come last
        /*
         * Todo: is switching the if/else more efficient?
         */
        public string getFixString()
        {
            List<string> dictList = new List<string>();

            foreach(KeyValuePair<int, string> kvp in _fixDict)
            {
                string listElement = kvp.Key.ToString() + "=" + kvp.Value;

                if (kvp.Key == 8 || kvp.Key == 9 || kvp.Key == 35 || kvp.Key == 10)
                {
                    continue;
                }
                else
                {
                    dictList.Add(listElement);
                }
            }
            dictList.Insert(0, "8=" + _fixDict[8]);
            dictList.Insert(1, "9=" + _fixDict[9]);
            dictList.Insert(2, "35=" + _fixDict[35]);
            dictList.Add("10=" + _fixDict[10]);

            string fixString = String.Join("|", dictList.ToArray());

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
            _fixDict[9] = findFixStringLength().ToString();
        }

        //This is bugged. It does not account for edge cases eg where adding a [9] element would increase the length from 99 to 100
        private int findFixStringLength()
        {
            int length = 0;
            foreach (KeyValuePair<int, string> kvp in _fixDict)
            {
                length += kvp.Key.ToString().Length;
                length += kvp.Value.Length;
                length += 1; //for the equals sign
            }

            length += _fixDict.Count() - 1;

            return length;
        }

        public int getHeartBeatInterval()
        {
            return int.Parse(_fixDict[108]);
        }
        public void setHeartBeatInterval(int interval)
        {
            //this._fixDict[108] = interval.ToString();
            addToFixMsg("108=" + interval.ToString());
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
