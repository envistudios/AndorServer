using System;
using System.Collections.Generic;
using MMO.Photon.Server;
using MMO.Photon.Application;
using MMO.Framework;
using AndorServerCommon;
using MMO.Photon.Client;
using SubServerCommon;
using Photon.SocketServer;
using SubServerCommon.Data.ClientData;

namespace AndorServer.Handlers
{
    public class SelectCharacterResponseHandler : PhotonServerHandler
    {
        public SelectCharacterResponseHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Response; }
        }

        public override byte Code
        {
            get { return (byte)ClientOperationCode.Login; }
        }

        public override int? SubCode
        {
            get { return (byte)MessageSubCode.SelectCharacter; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            if (message.Parameters.ContainsKey((byte)ClientParameterCode.PeerId))
            {
                PhotonClientPeer peer;
                Server.ConnectionCollection<AndorServerConnectionCollection>().
                    Clients.TryGetValue(new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]), out peer);

                if (peer != null)
                {
                    int characterId = Convert.ToInt32(message.Parameters[(byte)ClientParameterCode.CharacterId]);
                    peer.ClientData<CharacterData>().CharacterId = characterId;

                    var para = new Dictionary<byte, object>()
                    {
                        {(byte)ClientParameterCode.CharacterId, characterId},
                        {(byte)ClientParameterCode.PeerId, new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]).ToByteArray()},
                        {(byte)ClientParameterCode.UserId, peer.ClientData<CharacterData>().UserId}
                    };

                    var chatServer = Server.ConnectionCollection<AndorServerConnectionCollection>().OnGetServerByType((int)ServerType.Chat);

                    if (chatServer != null)
                    {
                        chatServer.SendEvent(new EventData((byte)ServerEventCode.CharacterRegister)
                        {
                            Parameters = para
                        }, new SendParameters());
                    }

                    //TODO: Add code to send some 
                    //event to region server
                    var regionServer = Server.ConnectionCollection<AndorServerConnectionCollection>().OnGetServerByType((int)ServerType.Region);

                    if (regionServer != null)
                    {
                        peer.CurrentServer = regionServer;

                        regionServer.SendEvent(new EventData((byte)ServerEventCode.CharacterRegister)
                        {
                            Parameters = para
                        }, new SendParameters());
                    }

                    // Security Measure
                    message.Parameters.Remove((byte)ClientParameterCode.PeerId);
                    message.Parameters.Remove((byte)ClientParameterCode.UserId);
                    message.Parameters.Remove((byte)ClientParameterCode.CharacterId);

                    var response = message as PhotonResponse;

                    if (response != null)
                    {
                        peer.SendOperationResponse(new OperationResponse(response.Code, response.Parameters)
                        {
                            DebugMessage = response.DebugMessage,
                            ReturnCode = response.ReturnCode
                        }, new SendParameters());
                    }
                    else
                    {
                        peer.SendOperationResponse(new OperationResponse(message.Code, message.Parameters), new SendParameters());
                    }
                }
            }

            return true;
        }
    }
}