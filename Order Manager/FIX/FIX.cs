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
            string versionRaw = _fixDict[8];
            string byteLengthRaw = "";
            if (_fixDict.ContainsKey(9))
            {
                byteLengthRaw = _fixDict[9];
            }
            string msgTypeRaw = _fixDict[35];
            string checksumRaw = _fixDict[10];

            string version = "8=" + versionRaw + "|";
            string byteLength = "";
            if (_fixDict.ContainsKey(9))
            {
                byteLength = "9=" + byteLengthRaw + "|";
            }
            string msgType = "35=" + msgTypeRaw + "|";
            string checksum = "|10=" + checksumRaw;

            _fixDict.Remove(8);
            _fixDict.Remove(9);
            _fixDict.Remove(35);
            _fixDict.Remove(10);

            string bulk = string.Join("|", _fixDict.Select(x => x.Key + "=" + x.Value));

            _fixDict[8] = versionRaw;
            _fixDict[9] = byteLengthRaw;
            _fixDict[35] = msgTypeRaw;
            _fixDict[10] = checksumRaw;

            string fixString = version + byteLength + msgType + bulk + checksum;
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
    }
}
