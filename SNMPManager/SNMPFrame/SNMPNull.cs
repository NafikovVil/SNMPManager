using System;
using System.Collections.Generic;
using System.Text;

namespace SNMPManager.SNMPFrame
{
    class SNMPNull: ISnmpVar
    {
        private const byte SNMPnull = 0x05; //начало int
        
        public int Length = 2;        

        public List<byte> Get() => new List<byte> { SNMPnull, 0};
        
    }
}
