﻿using System;
using MMO.Photon.Server;
using MMO.Photon.Application;
using MMO.Framework;
using AndorServerCommon;
using SubServerCommon;

namespace LoginServer.Handlers
{
    public class LoginServerUserLoggedOutHandler : PhotonServerHandler
    {
        public LoginServerUserLoggedOutHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte)ServerEventCode.UserLoggedOut; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            LoginServer server = Server as LoginServer;

            if (server != null)
            {
                Guid peerId = new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);
                server.ConnectionCollection<SubServerConnectionCollection>().Clients.Remove(peerId);
            }

            return true;
        }
    }
}