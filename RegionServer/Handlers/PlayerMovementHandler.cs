using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using AndorServerCommon;
using AndorServerCommon.MessageObjects;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;
using Photon.SocketServer;
using RegionServer.Model;
using RegionServer.Operations;

namespace RegionServer.Handlers
{
    public class PlayerMovementHandler : PhotonServerHandler
    {
        public PlayerMovementHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte) ClientOperationCode.Region; }
        }

        public override int? SubCode
        {
            get { return (int) MessageSubCode.PlayerMovement; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var para = new Dictionary<byte, object>
            {
                {(byte)ClientParameterCode.PeerId, message.Parameters[(byte)ClientParameterCode.PeerId]},
                {(byte)ClientParameterCode.SubOperationCode, message.Parameters[(byte)ClientParameterCode.SubOperationCode]}
            };

            var operation = new PlayerMovementOperation(serverPeer.Protocol, message);

            if (!operation.IsValid)
            {
                Log.ErrorFormat(operation.GetErrorMessage());
                serverPeer.SendOperationResponse(new OperationResponse(message.Code)
                {
                    ReturnCode = (int) ErrorCode.OperationInvalid,
                    DebugMessage = operation.GetErrorMessage(),
                    Parameters = para
                }, new SendParameters());

                return true;
            }

            Guid peerId = new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);
            var clients = Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<CPlayerInstance>();

            XmlSerializer mySerializer = new XmlSerializer(typeof(PlayerMovement));
            StringReader inString = new StringReader(operation.PlayerMovement);
            var playerMovement = (PlayerMovement) mySerializer.Deserialize(inString);

            instance.Physics.Movement = playerMovement;
            instance.Facing = playerMovement.Facing;

            return true;
        }
    }
}