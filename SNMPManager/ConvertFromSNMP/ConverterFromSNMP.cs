using System;
using System.Collections.Generic;
using System.Text;

namespace SNMPManager.ConvertFromSNMP
{

    public static class ConverterFromSNMP
    {
        public static int ConvertInt(List<byte> bytes)
        {
            int result = 0;
            int power = bytes.Count - 1;
            foreach (byte byt in bytes)
            {
                result += byt * (int)Math.Pow(256, power);
                power--;
            }
            return result.ToString();
        }

        public static string ConvertString(List<byte> bytes)
        {
            string result;
            Encoding ascii = Encoding.ASCII;
            result = ascii.GetString(bytes.ToArray());
            return result;
        }

        public static string ConvertNull(List<byte> bytes)
        {
            return "null";
        }

        public static string ConvertObject(List<byte> bytes)
        {
            StringBuilder result = new StringBuilder();
            byte byt1 = (byte)(bytes[0] / 40);
            result.Append(byt1 + ".");
            byte byt2 = (byte)(bytes[0] % 40);
            result.Append(byt2 + ".");
            bool isLongAddress = false;
            int longAddress = 0;
            for (int i = 1; i < bytes.Count; i++)
            {
                if ((bytes[i] & 0x80) != 0)
                {
                    isLongAddress = true;
                    longAddress |= (bytes[i] & 127);
                    longAddress <<= 7;
                }
                else if ((bytes[i] & 128) == 0 && isLongAddress)
                {
                    isLongAddress = false;
                    longAddress |= bytes[i];
                    result.Append(longAddress + ".");
                    longAddress = 0;
                }
                else
                    result.Append(bytes[i] + ".");
            }
            result.Remove(result.Length - 1, 1);
            return result.ToString();
        }

        public static string Convert(byte type, List<byte> bytes)
        {
            switch (type)
            {
                case 0x02:
                    return ConvertInt(bytes).ToString();
                case 0x05:
                    return ConvertNull(bytes);
                case 0x06:
                    return ConvertObject(bytes);
                case 0x04:
                    return ConvertString(bytes);
                default:
                    throw new NotSupportedException("Не известна тип переменной");
            }
        }
    }
}
