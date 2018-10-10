using System;
using System.Collections.Generic;
using System.Text;

namespace SNMPManager
{
    public enum TypeMibVar { Int, String, Obj, Null}

    public class AddrAndVar
    {
        public string MibAddr { get; set; }
        public TypeMibVar Type { get; set; }
        public string Var { get; set; }
    }
}
