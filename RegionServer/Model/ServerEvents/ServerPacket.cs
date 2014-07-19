using System.Collections.Generic;
using MMO.Photon.Application;
using AndorServer;
using AndorServerCommon;
using System.Xml.Serialization;
using System.IO;

namespace RegionServer.Model.ServerEvents
{
    public class ServerPacket : PhotonEvent
    {
        public bool SendToSelf { get; set; }

        public ServerPacket(ClientEventCode code, MessageSubCode subCode, bool sendToSelf = true) 
            : base((byte)code, (int?)subCode, new Dictionary<byte,object>())
        {
            SendToSelf = sendToSelf;
            AddParameter(subCode, ClientParameterCode.SubOperationCode);
        }

        public void AddParameter<T>(T obj, ClientParameterCode code)
        {
            if (Parameters.ContainsKey((byte) code))
            {
                Parameters[(byte) code] = obj;
            }
            else
            {
                Parameters.Add((byte)code, obj);
            }           
        }

        public void AddSerializedParameter<T>(T obj, ClientParameterCode code, bool binary = true)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringWriter outString = new StringWriter();
            serializer.Serialize(outString, obj);

            if (Parameters.ContainsKey((byte)code))
            {
                Parameters[(byte)code] = outString.ToString();
            }
            else
            {
                Parameters.Add((byte)code, outString.ToString());
            }    
        }
    }
}