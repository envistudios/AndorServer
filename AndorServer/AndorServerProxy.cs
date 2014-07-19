using System;
using System.Net;

using MMO.Photon.Application;
using AndorServerCommon;
using MMO.Photon.Server;
using Autofac;
using AndorServer.Handlers;
using MMO.Photon.Client;
using SubServerCommon.Data.ClientData;

namespace AndorServer
{
    public class AndorServerProxy : PhotonApplication
    {
        #region Implementation of PhotonApplication

        public override byte SubCodeParameterCode
        {
            get { return (byte)ClientParameterCode.SubOperationCode; }
        }

        public override IPEndPoint MasterEndPoint
        {
            get { throw new NotImplementedException(); }
        }

        public override int? TcpPort
        {
            get { throw new NotImplementedException(); }
        }

        public override int? UdpPort
        {
            get { throw new NotImplementedException(); }
        }

        public override IPAddress PublicIpAddress
        {
            get { throw new NotImplementedException(); }
        }

        public override int ServerType
        {
            get { throw new NotImplementedException(); }
        }

        protected override int ConnectRetryIntervalSeconds
        {
            get { throw new NotImplementedException(); }
        }

        protected override bool ConnectsToMaster
        {
            get { return false; }
        }

        protected override void RegisterContainerObjects(ContainerBuilder builder)
        {
            builder.RegisterType<AndorServerConnectionCollection>().As<PhotonConnectionCollection>().SingleInstance();
            builder.RegisterInstance(this).As<PhotonApplication>().SingleInstance();
            builder.RegisterType<CharacterData>().As<MMO.Framework.IClientData>();
            builder.RegisterType<EventForwardHandler>().As<DefaultEventHandler>().SingleInstance();
            builder.RegisterType<ResponseForwardHandler>().As<DefaultResponseHandler>().SingleInstance();
            builder.RegisterType<RequestForwardHandler>().As<DefaultRequestHandler>().SingleInstance();
            builder.RegisterType<HandleServerRegistration>().As<PhotonServerHandler>().SingleInstance();

            // Handlers
            builder.RegisterType<HandleClientLoginRequests>().As<PhotonClientHandler>().SingleInstance();
            builder.RegisterType<HandleClientChatRequests>().As<PhotonClientHandler>().SingleInstance();
            builder.RegisterType<HandleClientRegionRequests>().As<PhotonClientHandler>().SingleInstance();
            builder.RegisterType<LoginResponseHandler>().As<PhotonServerHandler>().SingleInstance();
            builder.RegisterType<SelectCharacterResponseHandler>().As<PhotonServerHandler>().SingleInstance();
        }

        protected override void ResolveParameters(IContainer container)
        {
        }

        public override void Register(PhotonServerPeer peer)
        {           
        }

        #endregion
    }
}