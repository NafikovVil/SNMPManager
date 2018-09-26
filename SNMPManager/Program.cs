using System;
using System.Net;

namespace SNMPManager
{
    class Program
    {
        static void Main(string[] args)
        {
            SNMPManager manager = new SNMPManager(0, "private", IPAddress.Parse("127.0.0.1"));
            manager.GetRequest("1.3.6.1.4.1.2680.1.2.7.3.2.0");
        }
    }
}
