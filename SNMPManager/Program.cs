using System;
using System.Net;

namespace SNMPManager
{
    class Program
    {
        static void Main(string[] args)
        {
            SNMPManager manager = new SNMPManager(pass: "public", addr: IPAddress.Parse("127.0.0.1"));
            manager.SetRequest(new AddrAndVar { MibAddr = "1.3.6.1.4.1.2680.1.2.7.3.2.0", Type = TypeMibVar.Int, Var = "3" }); //
            Console.ReadKey();
        }
    }
}
