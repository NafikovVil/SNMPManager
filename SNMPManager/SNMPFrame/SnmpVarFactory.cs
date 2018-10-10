using System;
using System.Collections.Generic;
using System.Text;

namespace SNMPManager.SNMPFrame
{
    static public class SnmpVarFactory
    {
        static public ISnmpVar CreateVar(TypeMibVar type, string var)
        {
            switch (type)
            {
                case TypeMibVar.Int:
                    return new SNMPInteger(Convert.ToInt32(var));
                case TypeMibVar.String:
                    return new SNMPString(var);
                case TypeMibVar.Obj:
                    return new SNMPObject(var);
                default:
                    return new SNMPNull();
            }
            
        }
    }
}
