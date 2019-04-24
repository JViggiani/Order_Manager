using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Order_Manager.FIX
{
    // Concrete Creators
    public static class FixFactory
    {
        public static Fix getFix(string msgType)
        {
            if (msgType == "0")
            {
                return new FixHeartbeat();
            }  
            else
                return new FixMessage("40="+msgType);
        }
    }

    public abstract class Fix : IFix
    {
        protected IDictionary<int, string> _fixDict;

        //Objective: get the fix msg as a string. 8 must come first. 9 must come second. 35 must come third. 10 must come last
        /*
         * Todo: is switching the if/else more efficient?
         */
        public string getFixString()
        {
            List<string> dictList = new List<string>();

            foreach (KeyValuePair<int, string> kvp in _fixDict)
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

        public void wrapMessageHeaderTrailer(string senderId, string targetId, int seqNum, string sendingTime)   // \todo this should only be performed ONCE. Just before it is sent... can approximate by putting it in getFixString
        {
            _fixDict[49] = senderId;
            _fixDict[56] = targetId;
            _fixDict[8] = "FIX.4.4";
            _fixDict[10] = "CHK";   //TODO placeholder checksum
            _fixDict[34] = seqNum.ToString();
            _fixDict[52] = sendingTime;
            calculateNewFixStringLength();
        }

        //This is bugged. It does not account for edge cases eg where adding a [9] element would increase the length from 99 to 100
        public int getFixLength()
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

        public string getTargetId()
        {
            return _fixDict[56];
        }
        public void setTargetId(string targetId)
        {
            _fixDict[56] = targetId;
        }

        protected void calculateNewFixStringLength()
        {
            //if(getFixLength() != Convert.ToInt32(_fixDict[9]))
            //{
                _fixDict[9] = getFixLength().ToString();
            //}
        }
    }

    public class FixMessage : Fix, IFix
    {
        public FixMessage(string fixString)
        {
            this.setFixString(fixString);
        }
        
        public void setFixString(string fixString)
        {
            _fixDict = fixString.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(part => part.Split('='))
               .ToDictionary(split => int.Parse(split[0]), split => split[1]);
        }

        public void addToFixMsg(string fixAppend)
        {
            string[] elementArray = fixAppend.Split("|");
            foreach(string element in elementArray)
            {
                string[] individualElement = element.Split("=");
                _fixDict[int.Parse(individualElement[0])] = individualElement[1];
            }
        }
    }

    /*
     * when either end of the connection has not sent any data for heartbtint seconds, it transmits this message
     * when either end of the connection has not recieved any data for heartbtint seconds + a reasonable transmission time, it transmits a test request msg
     * 
     * 
     */
    public class FixHeartbeat : Fix
    {
        public FixHeartbeat()
        {
            this._fixDict = new Dictionary<int, string>();
            //set all the needed attributes here. super simple msg
            this._fixDict[35] = "0";
        }   
    }

    public class FixLogon : Fix
    {
        public FixLogon(string heartBtInt)
        {
            this._fixDict = new Dictionary<int, string>();
            //set all the needed attributes here. super simple msg
            this._fixDict[35] = "A";
            this._fixDict[108] = heartBtInt;
        }
    }
}
