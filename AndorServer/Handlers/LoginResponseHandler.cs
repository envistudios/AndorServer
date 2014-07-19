using System;
using MMO.Photon.Server;
using MMO.Photon.Application;
using MMO.Framework;
using AndorServerCommon;
using MMO.Photon.Client;
using Photon.SocketServer;
using SubServerCommon.Data.ClientData;

namespace AndorServer.Handlers
{
    public class LoginResponseHandler : PhotonServerHandler
    {
        public LoginResponseHandler(PhotonApplication application)
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
            get { return (int)MessageSubCode.Login; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.DebugFormat("LoginResponseHandler.OnHandleMessage");

            if (message.Parameters.ContainsKey((byte)ClientParameterCode.PeerId))
            {
                PhotonClientPeer peer;
                Server.ConnectionCollection<PhotonConnectionCollection>().Clients.TryGetValue(
                    new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]), out peer);

                if (peer != null)
                {
                    if (message.Parameters.ContainsKey((byte)ClientParameterCode.UserId))
                    {
                        Log.DebugFormat("Found User {0}", Convert.ToInt32(message.Parameters[(byte)ClientParameterCode.UserId]));
                        peer.ClientData<CharacterData>().UserId = Convert.ToInt32(message.Parameters[(byte)ClientParameterCode.UserId]);
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