using System;
using System.Collections.Generic;
using System.Text;

namespace SNMPManager.SNMPFrame
{
    class SNMPNull
    {
        private const byte SNMPnull = 0x05; //начало int
        
        public int Length = 2;        

        public List<byte> GetNull() => new List<byte> { SNMPnull, 0};
        
    }
}
