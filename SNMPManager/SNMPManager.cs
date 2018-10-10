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

        public SNMPManager(string pass, IPAddress addr, int vers = 1)
        {
            version = vers - 1;
            password = pass;
            address = addr;
        }      

        public async Task<byte[]> GetRequest(params AddrAndVar[] mibVar)
        {
            SNMPSequence varbindList = MakeVarbindList(mibVar);
            SNMPSequence frame = MakeFrame(varbindList, SequenceType.PduGetRequest);

            // Отправка сообщения и ожидание ответа
            // Если сообщение не пришло в течении 5сек, то выкидывается ошибка
            byte[] frameByte = frame.Get().ToArray();
            UdpClient client = new UdpClient("127.0.0.1", 161);
            await client.SendAsync(frameByte, frameByte.Length);

            var receive = client.ReceiveAsync();
            var timeout = Task.Delay(5000);

            var completed = await Task.WhenAny(receive, timeout);
            if (completed == timeout)
            {
                Console.WriteLine("Сообщение не получено");
                return null;
            }
            var taskReceive = await receive;
            return taskReceive.Buffer;
        }

        public async Task<byte[]> SetRequest(params AddrAndVar[] mibVar)
        {
            SNMPSequence varbindList = MakeVarbindList(mibVar);
            SNMPSequence frame = MakeFrame(varbindList, SequenceType.PduSetRequest);

            // Отправка сообщения и ожидание ответа
            // Если сообщение не пришло в течении 5сек, то выкидывается ошибка
            byte[] frameByte = frame.Get().ToArray();
            UdpClient client = new UdpClient("127.0.0.1", 161);
            await client.SendAsync(frameByte, frameByte.Length);

            var receive = client.ReceiveAsync();
            var timeout = Task.Delay(5000);

            var completed = await Task.WhenAny(receive, timeout);
            if (completed == timeout)
            {
                Console.WriteLine("Сообщение не получено");
                return null;
            }
            var taskReceive = await receive;
            return taskReceive.Buffer;
        }

        private SNMPSequence MakeVarbindList(AddrAndVar[] mibVar)
        {
            List<byte> varbindTypes = new List<byte>();

            // Формирование VarBindType
            foreach (var mib in mibVar)
            {
                SNMPObject obj = new SNMPObject(mib.MibAddr);
                ISnmpVar var = SnmpVarFactory.CreateVar(mib.Type, mib.Var);                    
                SNMPSequence sequence = new SNMPSequence(SequenceType.Structure, obj.Get(), var.Get());
                varbindTypes.AddRange(sequence.Get());
            }

            SNMPSequence varbindList = new SNMPSequence(SequenceType.Structure, varbindTypes);
            return varbindList;
        }

        private SNMPSequence MakeFrame(SNMPSequence varbindList, SequenceType pduType)
        {
            // Добавление случайного индификатора сообщения
            var random = new Random();
            int idFrame;
            idFrame = 1;//random.Next();
            SNMPInteger id = new SNMPInteger(idFrame);
            // Добавление ошибки сообщения
            SNMPInteger error = new SNMPInteger(0);
            // Добавление индекса ошибки сообщения
            SNMPInteger errorIndex = new SNMPInteger(0);
            // Добавление PDUType
            List<byte> SNMPPdu = new List<byte>();
            SNMPSequence Pdu = new SNMPSequence(pduType, id.Get(),
                error.Get(), errorIndex.Get(), varbindList.Get());

            // Добавление версии
            SNMPInteger vers = new SNMPInteger(version);
            // Добавление пароля
            SNMPString pass = new SNMPString(password);
            // Создание кадра
            SNMPSequence frame = new SNMPSequence(SequenceType.Structure, vers.Get(), pass.Get(), Pdu.Get());
            return frame;
        }

        /*private SNMPSequence MakeVarbindList(string[] mibVar)
        {
            List<byte> varbindTypes = new List<byte>();

            // Формирование VarBindType
            foreach (var mib in mibVar)
            {
                SNMPObject obj = new SNMPObject(mib);
                SNMPNull nul = new SNMPNull();
                SNMPSequence sequence = new SNMPSequence(SequenceType.Structure, obj.Get(), nul.Get());
                varbindTypes.AddRange(sequence.Get());
            }

            SNMPSequence varbindList = new SNMPSequence(SequenceType.Structure, varbindTypes);
            return varbindList;
        }*/
    }
}
