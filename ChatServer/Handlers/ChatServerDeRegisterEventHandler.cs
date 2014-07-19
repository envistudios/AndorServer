using System;
using MMO.Photon.Server;
using MMO.Photon.Application;
using MMO.Framework;
using SubServerCommon;
using AndorServerCommon;

namespace ChatServer.Handlers
{
    public class ChatServerDeRegisterEventHandler : PhotonServerHandler
    {
        public ChatServerDeRegisterEventHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte)ServerEventCode.CharacterDeRegister; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Guid peerId = new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);

            // remove from groups, guilds, etc

            Server.ConnectionCollection<SubServerConnectionCollection>().Clients.Remove(peerId);

            Log.DebugFormat("Removed Peer {0}, Now we have {1} clients",
                            peerId, Server.ConnectionCollection<SubServerConnectionCollection>().Clients.Count);

            return true;
        }
    }
}