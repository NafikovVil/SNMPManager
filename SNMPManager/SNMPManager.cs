using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SNMPManager.ConvertFromSNMP;
using SNMPManager.SNMPFrame;

namespace SNMPManager
{
    public class SNMPManager
    {        
        private int version;
        private string password;
        private IPEndPoint address;

        public SNMPManager(string pass, IPAddress addr, int vers = 1)
        {
            version = vers - 1;
            password = pass;
            address = new IPEndPoint(addr, 1024);             
        }      

        public async Task<byte[]> GetRequest(params AddrAndVar[] mibVar)
        {
            SNMPSequence varbindList = MakeVarbindList(mibVar);
            int idFrame = GetIdMes();
            SNMPSequence frame = MakeFrame(idFrame, varbindList, SequenceType.PduGetRequest);

            // Отправка сообщения и ожидание ответа
            UdpClient client = new UdpClient();
            client.Connect(address);
            await SendFrame(client, frame);
            
            UdpReceiveResult taskReceive = await ReceiveFrame(client);
            MakeResponce(idFrame, taskReceive.Buffer);
            return taskReceive.Buffer;
        }        

        public async Task<byte[]> GetNext(params AddrAndVar[] mibVar)
        {
            SNMPSequence varbindList = MakeVarbindList(mibVar);
            int idFrame = GetIdMes();
            SNMPSequence frame = MakeFrame(idFrame, varbindList, SequenceType.PduGetNext);

            // Отправка сообщения и ожидание ответа
            UdpClient client = new UdpClient();
            client.Connect(address);
            await SendFrame(client, frame);

            UdpReceiveResult taskReceive = await ReceiveFrame(client);
            return taskReceive.Buffer;
        }

        public async Task<byte[]> SetRequest(params AddrAndVar[] mibVar)
        {
            SNMPSequence varbindList = MakeVarbindList(mibVar);
            int idFrame = GetIdMes();
            SNMPSequence frame = MakeFrame(idFrame, varbindList, SequenceType.PduSetRequest);

            // Отправка сообщения и ожидание ответа
            UdpClient client = new UdpClient();
            client.Connect(address);
            await SendFrame(client, frame);

            UdpReceiveResult taskReceive = await ReceiveFrame(client);
            return taskReceive.Buffer;
        }

        private List<AddrAndVar> MakeResponce(int id, byte[] responce)
        {
            const int indexLengthPass = 6;
            const int indexId = 2 + 3 + 2 + 2;
            const int lengthError = 1;
            if (responce == null)
                throw new ArgumentNullException();
            if (responce[1] != responce.Length - 2)
                throw new Exception("Неправильная длина сообщения");
            List<byte> responceList = new List<byte>(responce);

            int lengthPass = responceList[indexLengthPass];
            int indexReqId = indexId + lengthPass,
                lengthId = responceList[indexReqId + 1];

            int requestId = ConverterFromSNMP.ConvertInt(responceList.GetRange(indexReqId + 2, lengthId));
            if (requestId != id)
            {
                throw new Exception("ID не совпадают");
            }
            int indexError = indexReqId + lengthId + 1 + 1;
            int error = ConverterFromSNMP.ConvertInt(responceList.GetRange(indexError + 2, lengthError));

            int indexErrorIndex = indexError + 3,
                lengthErrorIndex = responceList[indexErrorIndex + 1];
            int errorIndex = ConverterFromSNMP.ConvertInt(responceList.GetRange(indexErrorIndex + 2, lengthErrorIndex));
            ProduceError(error, errorIndex);

            int indexVarbindList = indexErrorIndex + 3;
            int lengthVarbindList = responceList[indexVarbindList + 1];
            int lengthList = 0;
            int indexVar = indexVarbindList + 2;
            while(lengthList < lengthVarbindList)
            {
                int lengthAddr = responceList[indexVar + 1];
                lengthList += lengthAddr;
                string Addr = ConverterFromSNMP.ConvertObject(responceList.GetRange(indexVar + 2, lengthAddr));

            }

            return new List<AddrAndVar>();
        }

        private void ProduceError(int error, int errorIndex)
        {
            switch (error)
            {
                case 1:
                    throw new Exception($"Response message too large to transport in {errorIndex}");
                case 2:
                    throw new Exception($"The name of the requested object was not found in {errorIndex}");
                case 3:
                    throw new Exception($"A data type in the request did not match the data type in the SNMP agent in {errorIndex}");
                case 4:
                    throw new Exception($"The SNMP manager attempted to set a read-only parameter in {errorIndex}");
                case 5:
                    throw new Exception("General Error (some error other than the ones listed above)");
                default:
                    return;
            }
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

        private int GetIdMes()
        {
            var random = new Random();
            return random.Next();
        }

        private SNMPSequence MakeFrame(int idFrame, SNMPSequence varbindList, SequenceType pduType)
        {
            // Добавление случайного индификатора сообщения
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

        private async Task SendFrame(UdpClient client, SNMPSequence frame)
        {
            byte[] frameByte = frame.Get().ToArray();
            await client.SendAsync(frameByte, frameByte.Length);
        }

        private async Task<UdpReceiveResult> ReceiveFrame(UdpClient client)
        {
            var receive = client.ReceiveAsync();
            var timeout = Task.Delay(5000);
            var completed = await Task.WhenAny(receive, timeout);
            if (completed == timeout)
            {
                throw new Exception("Сообщение не получено");
            }
            var taskReceive = await receive;
            return taskReceive;
        }
    }
}
