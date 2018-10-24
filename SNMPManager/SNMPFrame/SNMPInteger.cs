using SNMPManager.SNMPFrame;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMPManager.SNMPFrame
{
    public class SNMPInteger: ISnmpVar
    {
        private const byte integer = 0x02; //начало int

        private List<byte> result;
        private int number;
        
        public SNMPInteger(int numb)
        {
            number = numb;
        }

        public List<byte> Get()
        {
            MakeInt();
            return result;
        }

        private void MakeInt()
        {
            List<byte> byteNumber = GetBytes(number);
            result = new List<byte>
            {
                integer,
                (byte)byteNumber.Count
            };
            result.AddRange(byteNumber);
        }

        private List<byte> GetBytes(int numb)
        {
            List<byte> bytes = new List<byte>();           
            uint number = (uint)numb;
            while (number != 0)
            {
                bytes.Add((byte)(number & 0xFF));
                number >>= 8;
            }

            bytes.Reverse();

            return bytes;
        }
    }
}
