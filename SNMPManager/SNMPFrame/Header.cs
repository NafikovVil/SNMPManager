using SNMPManager.SNMPFrame;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNMPManager
{
    class Header
    {
        private const byte structure = 0x30; //начало заголовка

        private List<byte> header;

        public Header(int version, string password)
        {
            MakeHeader(version, password);
        }

        private void MakeHeader(int versionIn, string passwordIn)
        {
            header = new List<byte>();
            header.Add(structure);
            header.Add(0); //Длина еще не определена

            SNMPInteger version = new SNMPInteger(versionIn);
            header.AddRange(version.GetInt());

            SNMPString password = new SNMPString(passwordIn);
            header.AddRange(password.GetString());
            ;
        }
    }
}
