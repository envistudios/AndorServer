using System;
using System.Collections.Generic;
using MMO.Photon.Server;
using MMO.Photon.Application;
using MMO.Framework;
using AndorServerCommon;
using System.Xml.Serialization;
using AndorServerCommon.MessageObjects;
using System.IO;
using Photon.SocketServer;
using AndorServer;
using SubServerCommon.Data.ClientData;

namespace ChatServer.Handlers
{
    public class ChatServerChatMessageHandler : PhotonServerHandler
    {
        public ChatServerChatMessageHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte)ClientOperationCode.Chat; }
        }

        public override int? SubCode
        {
            get { return (byte)MessageSubCode.Chat; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            if (message.Parameters.ContainsKey((byte)ClientParameterCode.Object))
            {
                Guid peerId = new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);

                XmlSerializer mySerializer = new XmlSerializer(typeof(ChatItem));
                StringReader inString = new StringReader((string)message.Parameters[(byte)ClientParameterCode.Object]);

                var chatItem = (ChatItem)mySerializer.Deserialize(inString);

                switch (chatItem.Type)
                {
                    case ChatType.General:
                        chatItem.Text = string.Format("[General] {0}: {1}", 
                            Server.ConnectionCollection<SubServerConnectionCollection>().Clients[peerId].ClientData<ChatPlayer>().CharacterName, chatItem.Text);
                        StringWriter outString = new StringWriter();
                        mySerializer.Serialize(outString, chatItem);

                        foreach (var client in Server.ConnectionCollection<SubServerConnectionCollection>().Clients)
                        {
                            var para = new Dictionary<byte, object>
                            {
                                {(byte)ClientParameterCode.PeerId, client.Key.ToByteArray()},
                                {(byte)ClientParameterCode.Object, outString.ToString()}
                            };

                            EventData eventData = new EventData
                            {
                                Code = (byte)ClientEventCode.Chat,
                                Parameters = para
                            };

                            client.Value.ClientData<ServerData>().ServerPeer.SendEvent(eventData, new SendParameters());
                        }

                        break;

                    default:
                        break;
                }
            }

            return true;
        }
    }
}