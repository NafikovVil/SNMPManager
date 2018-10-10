using System;
using System.Collections.Generic;
using System.Text;

namespace SNMPManager.SNMPFrame
{
    public class SNMPString: ISnmpVar
    {
        private const byte octetString = 0x04; //начало строки

        private List<byte> result;
        private string str;

        public SNMPString(string str)
        {
            this.str = str;
        }

        public List<byte> Get() {
            MakeString(str);
            return result;
        }

        private void MakeString(string str)
        {
            List<byte> byteString = GetBytes(str);
            result = new List<byte>();
            result.Add(octetString);
            result.Add((byte)byteString.Count);
            result.AddRange(byteString);
        }

        private List<byte> GetBytes(string str)
        {
            List<byte> bytes = new List<byte>();
            if (str.Length == 0)
            {
                bytes.Add(0);
                return bytes;
            }

            Encoding ascii = Encoding.ASCII;
            Byte[] encodedBytes = ascii.GetBytes(str);

            bytes.AddRange(encodedBytes);
            return bytes;
        }
    }
}
