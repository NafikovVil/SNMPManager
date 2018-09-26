using System;
using System.Collections.Generic;
using System.Text;

namespace SNMPManager.SNMPFrame
{
    public class SNMPString
    {
        private const byte octetString = 0x04; //начало строки

        private List<byte> result;

        public int Length { get; }

        public SNMPString(string str)
        {
            MakeString(str);
            Length = result.Count;
        }

        public List<byte> GetString() => result;

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
