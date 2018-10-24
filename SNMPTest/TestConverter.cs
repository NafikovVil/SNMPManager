using SNMPManager;
using SNMPManager.ConvertFromSNMP;
using SNMPManager.SNMPFrame;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SNMPTest
{
    public class TestConverter
    {
        [Fact]
        public void Test1()
        {
            int orig = 2147483647;
            SNMPInteger snmpInteger = new SNMPInteger(orig);
            List<byte> bytes = snmpInteger.Get();
            bytes.RemoveAt(0);
            bytes.RemoveAt(0);
            int aft = ConverterFromSNMP.ConvertInt(bytes);

            Assert.Equal(orig, aft);
        }

        [Fact]
        public void TestConvertString()
        {
            string orig = "test";
            SNMPString snmpString = new SNMPString(orig);
            List<byte> bytes = snmpString.Get();
            bytes.RemoveAt(0);
            bytes.RemoveAt(0);

            string aft = ConverterFromSNMP.ConvertString(bytes);

            Assert.Equal(orig, aft);
        }

        [Fact]
        public void TestConvertObject()
        {
            string orig = "1.3.4.5.6.7.128.25678";
            SNMPObject snmpObject = new SNMPObject(orig);
            List<byte> bytes = snmpObject.Get();
            bytes.RemoveAt(0);
            bytes.RemoveAt(0);

            string aft = ConverterFromSNMP.ConvertObject(bytes);

            Assert.Equal(orig, aft);
        }
    }
}
