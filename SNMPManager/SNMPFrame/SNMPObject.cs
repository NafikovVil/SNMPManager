using System;
using System.Collections.Generic;
using System.Text;

namespace SNMPManager.SNMPFrame
{
    public class SNMPObject: ISnmpVar
    {
        private const byte obj = 0x06; //начало obj

        private List<byte> result;
        private string mibAddr;        

        public SNMPObject(string mib)
        {
            mibAddr = mib;
        }

        public List<byte> Get()
        {
            MakeObject(mibAddr);
            return result;
        }

        private void MakeObject(string mib)
        {
            result = new List<byte> { obj, 0 };
            
            string[] num = mib.Split(".");
           
            // Первые два значения адреса обрабатываются по другому (1num * 40 + 2num)
            result.Add((byte) (Convert.ToByte(num[0]) * 40 + Convert.ToByte(num[1])));

            for (int index = 2; index < num.Length; index++)
            {
                if (Convert.ToInt32(num[index]) > 127)
                {
                    List<byte> longAddr = new List<byte>();
                    int tempNum = Convert.ToInt32(num[index]);
                    while (tempNum > 0)
                    {
                        longAddr.Add((byte)(tempNum & 127));
                        tempNum = tempNum >> 7;
                    }
                    for (int i = 1; i < longAddr.Count; i++)
                    {
                        longAddr[i] = (byte)(longAddr[i] | 128);
                    }
                    longAddr.Reverse();
                    result.AddRange(longAddr);
                }
                else
                {
                    result.Add(Convert.ToByte(num[index]));
                }
            }
            result[1] = (byte)(result.Count - 2);
        }        
    }
}
