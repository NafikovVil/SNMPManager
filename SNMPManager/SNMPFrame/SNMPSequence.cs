using System;
using System.Collections.Generic;
using System.Text;

namespace SNMPManager.SNMPFrame
{
    public enum SequenceType { Structure = 0x30, PduGetRequest = 0xA0, PduSetRequest = 0xA3 }

    class SNMPSequence : ISnmpVar
    {
        private SequenceType type;
        private List<byte> result;
        List<byte>[] bytes;

        public SNMPSequence(SequenceType type, params List<byte>[] bytes)
        {
            this.type = type;
            this.bytes = bytes;
        }

        public List<byte> Get()
        {
            MakeSequence(bytes);
            return result;
        }

        private void MakeSequence(List<byte>[] bytes)
        {
            int length = 0;
            result = new List<byte>();

            foreach(List<byte> listBytes in bytes)
            {
                length += listBytes.Count;
                result.AddRange(listBytes);
            }

            result.Insert(0, (byte)type);

            //обработка ошибки добавить!
            if (length < 256)
                result.Insert(1, (byte)length);
        }
    }
}
