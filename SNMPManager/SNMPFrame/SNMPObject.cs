using System;
using System.Collections.Generic;
using System.Text;

namespace SNMPManager.SNMPFrame
{
    class SNMPObject
    {
        private const byte obj = 0x06; //начало obj

        private List<byte> result;

        public int Length { get; }

        public SNMPObject(string mib)
        {
            MakeObject(mib);
        }

        public List<byte> GetObj() => result;

        private void MakeObject(string mib)
        {
            result = new List<byte> { obj, 0 };
            
            string[] num = mib.Split(".");

            //Первые два значения адреса обрабатываются по другому (1num * 40 + 2num)
            result.Add((byte) (Convert.ToByte(num[0]) * 40 + Convert.ToByte(num[1])));

            for (int index = 2; index < num.Length; index++)
            {
                if (num[index].Length > 1)
                {
                    int tempNum = Convert.ToInt32(num[index]);
                    result.Add((byte)(tempNum / 128 | 0x80));
                    result.Add((byte)(tempNum % 128));
                }
                else
                {
                    result.Add(Convert.ToByte(num[index]));
                }
            }

            result[1] = (byte)num.Length;
        }        
    }
}
