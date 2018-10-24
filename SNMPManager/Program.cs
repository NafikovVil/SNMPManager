using SNMPManager.ConvertFromSNMP;
using SNMPManager.SNMPFrame;
using System;
using System.Collections.Generic;
using System.Net;

namespace SNMPManager
{
    class Program
    {
        static void Main(string[] args)
        {
            NewMethod(); 
            Console.ReadKey();
        }

        private async static void NewMethod()
        {
            SNMPManager manager = new SNMPManager(pass: "public", addr: IPAddress.Parse("127.0.0.1"));
            await manager.GetRequest(new AddrAndVar { MibAddr = "1.3.6.1.2.1.9585.13.0", Type = TypeMibVar.Null, Var = "1" });
        }
    }
}
