using System;
using System.Collections.Generic;
using System.Text;

namespace SNMPManager.SNMPFrame
{
    public interface ISnmpVar
    {
        List<byte> Get();
    }
}
