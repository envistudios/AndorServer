﻿using System.Net;
using AndorServerCommon;
using Autofac;
using SubServerCommon.Handlers;
using MMO.Photon.Server;
using MMO.Photon.Application;
using MMO.Photon.Client;
using SubServerCommon.Data.ClientData;
using MMO.Framework;
using System.Reflection;
using SubServerCommon.Data;
using System.Xml.Serialization;
using System.IO;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.Operations;

namespace ChatServer
{
    public class ChatServer : PhotonApplication
    {
        private readonly IPAddress _publicIPAddress = IPAddress.Parse("127.0.0.1");
        private readonly IPEndPoint _masterEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4520);

        #region Implementation of PhotonApplication

        public override byte SubCodeParameterCode
        {
            get { return (byte)ClientParameterCode.SubOperationCode; }
        }

        public override IPEndPoint MasterEndPoint
        {
            get { return _masterEndPoint; }
        }

        public override int? TcpPort
        {
            get { return 4532; }
        }

        public override int? UdpPort
        {
            get { return 50567; }
        }

        public override IPAddress PublicIpAddress
        {
            get { return _publicIPAddress; }
        }

        public override int ServerType
        {
            get { return (int)SubServerCommon.ServerType.Chat; }
        }

        protected override int ConnectRetryIntervalSeconds
        {
            get { return 14; }
        }

        protected override bool ConnectsToMaster
        {
            get { return true; }
        }

        protected override void RegisterContainerObjects(ContainerBuilder builder)
        {
            builder.RegisterType<ErrorEventForwardHandler>().As<DefaultEventHandler>().SingleInstance();
            builder.RegisterType<ErrorRequestForwardHandler>().As<DefaultRequestHandler>().SingleInstance();
            builder.RegisterType<ErrorResponseForwardHandler>().As<DefaultResponseHandler>().SingleInstance();

            // Remove later I think look into thsi
            builder.RegisterType<SubServerConnectionCollection>().As<PhotonConnectionCollection>().SingleInstance();
            builder.RegisterInstance(this).As<PhotonApplication>().SingleInstance();
            builder.RegisterType<SubServerClientPeer>();
            builder.RegisterType<CharacterData>().As<IClientData>();
            builder.RegisterType<ChatPlayer>().As<IClientData>();
            builder.RegisterType<ServerData>().As<IClientData>();

            // Handlers
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(GetType())).Where(t => t.Name.EndsWith("Handler")).As<PhotonServerHandler>().SingleInstance();
        }

        protected override void ResolveParameters(IContainer container)
        {
        }

        public override void Register(PhotonServerPeer peer)
        {
            var registerSubServerOpertion = new RegisterSubServerData()
            {
                GameServerAddress = PublicIpAddress.ToString(),
                TcpPort = TcpPort,
                UdpPort = UdpPort,
                ServerId = ServerId,
                ServerType = ServerType,
                ApplicationName = ApplicationName
            };

            XmlSerializer mySerializer = new XmlSerializer(typeof(RegisterSubServerData));
            StringWriter outString = new StringWriter();
            mySerializer.Serialize(outString, registerSubServerOpertion);

            peer.SendOperationRequest(
                new OperationRequest((byte)ServerOperationCode.RegisterSubServer,
                                    new RegisterSubServer() { RegisterSubServerOperation = outString.ToString() }), new SendParameters());
        }

        #endregion
    }
}
