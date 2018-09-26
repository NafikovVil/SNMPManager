using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SNMPManager.SNMPFrame;

namespace SNMPManager
{
    public class SNMPManager
    {        
        private int version;
        private string password;
        private IPAddress address;

        public SNMPManager(int vers, string pass, IPAddress addr)
        {
            version = vers;
            password = pass;
            address = addr;
        }      

        public async void GetRequest(params string[] mibVar)
        {    
            List<byte> varbindTypes = new List<byte>();
            //Формирование VarBindType
            foreach(var mib in mibVar)
            {
                SNMPObject obj = new SNMPObject(mib);
                SNMPNull nul = new SNMPNull();
                SNMPSequence sequence = new SNMPSequence(SequenceType.Structure, obj.GetObj(), nul.GetNull());
                varbindTypes.AddRange(sequence.GetSequence());
            }

            SNMPSequence varbindList = new SNMPSequence(SequenceType.Structure, varbindTypes);

            var random = new Random();
            int idFrame;
            idFrame = 1;//random.Next();

            // Добавление случайного индификатора сообщения
            SNMPInteger id = new SNMPInteger(idFrame);
            // Добавление ошибки сообщения
            SNMPInteger error = new SNMPInteger(0);
            // Добавление индекса ошибки сообщения
            SNMPInteger errorIndex = new SNMPInteger(0);
            // Добавление PDUType
            List<byte> SNMPPdu = new List<byte>();
            SNMPSequence Pdu = new SNMPSequence(SequenceType.PduGetRequest, id.GetInt(),
                error.GetInt(), errorIndex.GetInt(), varbindList.GetSequence());

            //Добавление версии
            SNMPInteger vers = new SNMPInteger(version);
            // Добавление пароля
            SNMPString pass = new SNMPString(password);
            // Создание кадра
            SNMPSequence frame = new SNMPSequence(SequenceType.Structure, vers.GetInt(), pass.GetString(), Pdu.GetSequence());
            
            byte[] frameByte = frame.GetSequence().ToArray();
            UdpClient client = new UdpClient(161);
            await client.SendAsync(frameByte, frameByte.Length);

            var receive = client.ReceiveAsync();
            var timeout = Task.Delay(5000);

            var completed = await Task.WhenAny(receive, timeout);
            if (completed == timeout)
                Console.WriteLine("Сообщение не получено");


        }
    }
}
