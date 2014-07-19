using System;
using MMO.Photon.Server;
using MMO.Photon.Application;
using MMO.Framework;
using AndorServerCommon;
using RegionServer.Model;

namespace RegionServer.Handlers
{
    public class PlayerInGameHandler : PhotonServerHandler
    {
        public PlayerInGameHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte)ClientOperationCode.Region; }
        }

        public override int? SubCode
        {
            get { return (int)MessageSubCode.PlayerInGame; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Guid peerId = new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);

            var clients = Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
            var instance = clients[peerId].ClientData<CPlayerInstance>();

            instance.Spawn();
            instance.BroadcastUserInfo();
            Log.DebugFormat("Character in RegionServer");

            // Send notify messages guildies,
            // friends, etc..

            return true;
        }
    }
}