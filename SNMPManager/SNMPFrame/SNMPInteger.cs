using System;
using System.Collections.Generic;
using System.Text;

namespace SNMPManager
{
    public class SNMPInteger
    {
        private const byte integer = 0x02; //начало int

        private List<byte> result;

        public int Length { get; }

        public SNMPInteger(int number)
        {
            MakeInt(number);
            Length = result.Count;
        }

        public List<byte> GetInt() => result;

        private void MakeInt(int number)
        {
            List<byte> byteNumber = GetBytes(number);
            result = new List<byte>();
            result.Add(integer);
            result.Add((byte)byteNumber.Count);
            result.AddRange(byteNumber);
        }

        private List<byte> GetBytes(int number)
        {
            List<byte> bytes = new List<byte>();
            if (number == 0)
            {
                bytes.Add(0);
                return bytes;
            }
            
            while (number != 0)
            {
                bytes.Add((byte)(number % 256));
                number /= 256;
            }

            bytes.Reverse();

            return bytes;
        }
    }
}
