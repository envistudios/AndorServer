using System;
using System.Collections.Generic;
using System.Linq;
using MMO.Photon.Application;
using MMO.Photon.Server;
using MMO.Photon.Client;
using Photon.SocketServer;
using AndorServerCommon;
using SubServerCommon;
using SubServerCommon.Data.ClientData;

namespace AndorServer
{
    public class AndorServerConnectionCollection : PhotonConnectionCollection
    {
        public PhotonServerPeer LoginServer { get; protected set; }
        public PhotonServerPeer ChatServer { get; protected set; }
        public PhotonServerPeer RegionServer { get; protected set; }

        public AndorServerConnectionCollection()
        {
            LoginServer = null;
            ChatServer = null;
            RegionServer = null;
        }

        public override void Disconnect(PhotonServerPeer serverPeer)
        {
            if (serverPeer.ServerId.HasValue)
            {
                if (ChatServer != null && serverPeer.ServerId.Value == ChatServer.ServerId)
                {
                    ChatServer = null;
                }

                if (LoginServer != null && serverPeer.ServerId.Value == LoginServer.ServerId)
                {
                    LoginServer = null;
                }

                if (RegionServer != null && serverPeer.ServerId.Value == RegionServer.ServerId)
                {
                    RegionServer = null;
                }
            }
        }

        public override void Connect(PhotonServerPeer serverPeer)
        {
            if ((serverPeer.ServerType & (int)ServerType.Region) != 0)
            {
                Dictionary<byte, object> parameters = new Dictionary<byte, object>();

                Dictionary<string, string> serverList = Servers.Where(
                    incomingSubServerPeer => 
                    incomingSubServerPeer.Value.ServerId.HasValue && 
                    !incomingSubServerPeer.Value.ServerId.Equals(serverPeer.ServerId) &&
                    (incomingSubServerPeer.Value.ServerType & (int)ServerType.Region) != 0)
                                                                .ToDictionary(
                                                                    incomingSubServerPeer => 
                                                                    incomingSubServerPeer.Value.ApplicationName, 
                                                                    incomingSubServerPeer => 
                                                                    incomingSubServerPeer.Value.TcpAddress);

                if (serverList.Count > 0)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Sending list of {0} connected sub servers", serverList.Count);
                    }

                    parameters.Add((byte)ServerParameterCode.SubServerDictionary, serverList);
                    serverPeer.SendEvent(new EventData((byte)ServerEventCode.SubServerList, parameters), new SendParameters());
                }
            }
        }

        public override void ClientConnect(PhotonClientPeer clientPeer)
        {
            if (clientPeer.ClientData<CharacterData>().CharacterId.HasValue)
            {
                var para = new Dictionary<byte, object>
                {
                    {(byte)ClientParameterCode.CharacterId, clientPeer.ClientData<CharacterData>().CharacterId.Value},
                    {(byte)ClientParameterCode.PeerId, clientPeer.PeerId}
                };

                if (ChatServer != null)
                {
                    ChatServer.SendEvent(new EventData((byte)ServerEventCode.CharacterRegister, para), new SendParameters());
                }

                if (clientPeer.CurrentServer != null)
                {
                    clientPeer.CurrentServer.SendEvent(new EventData((byte)ServerEventCode.CharacterRegister, para), new SendParameters());
                }
            }
        }

        public override void ClientDisconnect(PhotonClientPeer clientPeer)
        {
            var para = new Dictionary<byte, object> {{(byte)ClientParameterCode.PeerId, clientPeer.PeerId.ToByteArray()}};

            if (clientPeer.ClientData<CharacterData>().CharacterId.HasValue)
            {
                Log.DebugFormat("Trying to disconnect client {0}:{1}", clientPeer.PeerId, clientPeer.ClientData<CharacterData>().CharacterId.Value);

                if (ChatServer != null)
                {
                    ChatServer.SendEvent(new EventData((byte)ServerEventCode.CharacterDeRegister, para), new SendParameters());
                }

                if (clientPeer.CurrentServer != null)
                {
                    clientPeer.CurrentServer.SendEvent(new EventData((byte)ServerEventCode.CharacterDeRegister, para), new SendParameters());
                }
            }

            LoginServer.SendEvent(new EventData((byte)ServerEventCode.UserLoggedOut, para), new SendParameters());
        }

        public override void ResetServers()
        {
            if (ChatServer != null && ChatServer.ServerType != (int)ServerType.Chat)
            {
                PhotonServerPeer peer = Servers.Values.Where(subServerPeer => subServerPeer.ServerType == (int)ServerType.Chat).FirstOrDefault();

                if (peer != null)
                {
                    ChatServer = peer;
                }
            }

            if (LoginServer != null && LoginServer.ServerType != (int)ServerType.Login)
            {
                PhotonServerPeer peer = Servers.Values.Where(subServerPeer => subServerPeer.ServerType == (int)ServerType.Login).FirstOrDefault();

                if (peer != null)
                {
                    LoginServer = peer;
                }
            }

            if (RegionServer != null && RegionServer.ServerType != (int)ServerType.Region)
            {
                PhotonServerPeer peer = Servers.Values.Where(subServerPeer => subServerPeer.ServerType == (int)ServerType.Region).FirstOrDefault();

                if (peer != null)
                {
                    RegionServer = peer;
                }
            }

            if (ChatServer == null || ChatServer.ServerId == null)
            {
                ChatServer = Servers.Values.Where(subServerPeer => subServerPeer.ServerType == (int)ServerType.Chat).FirstOrDefault() ??
                    Servers.Values.Where(subServerPeer => (subServerPeer.ServerType & (int)ServerType.Chat) != 0).FirstOrDefault();
            }

            if (LoginServer == null || LoginServer.ServerId == null)
            {
                LoginServer = Servers.Values.Where(subServerPeer => subServerPeer.ServerType == (int)ServerType.Login).FirstOrDefault() ??
                    Servers.Values.Where(subServerPeer => (subServerPeer.ServerType & (int)ServerType.Login) != 0).FirstOrDefault();
            }

            if (RegionServer == null || RegionServer.ServerId == null)
            {
                RegionServer = Servers.Values.Where(subServerPeer => subServerPeer.ServerType == (int)ServerType.Region).FirstOrDefault() ??
                    Servers.Values.Where(subServerPeer => (subServerPeer.ServerType & (int)ServerType.Region) != 0).FirstOrDefault();
            }

            if (LoginServer != null)
            {
                Log.DebugFormat("Login Server: {0}", LoginServer.ConnectionId);
            }

            if (ChatServer != null)
            {
                Log.DebugFormat("Chat Server: {0}", ChatServer.ConnectionId);
            }

            if (RegionServer != null)
            {
                Log.DebugFormat("Region Server: {0}", RegionServer.ConnectionId);
            }
        }

        public override bool IsServerPeer(InitRequest initRequest)
        {
            Log.DebugFormat("Recieved Init request to {0}:{1} - {2}", initRequest.LocalIP, initRequest.LocalPort, initRequest);

            if (initRequest.LocalPort == 4520)
            {
                return true;
            }

            return false;
        }

        public override PhotonServerPeer OnGetServerByType(int serverType, params object[] additional)
        {
            PhotonServerPeer server = null;

            switch ((ServerType)Enum.ToObject(typeof(ServerType), serverType))
            {
                case ServerType.Login:
                    if (LoginServer != null)
                    {
                        Log.DebugFormat("Found Login Server");
                        server = LoginServer;
                    }
                    break;

                case ServerType.Chat:
                    if (ChatServer != null)
                    {
                        Log.DebugFormat("Found Chat Server");
                        server = ChatServer;
                    }
                    break;

                case ServerType.Region:
                    if (RegionServer != null)
                    {
                        Log.DebugFormat("Found Region Server");
                        server = RegionServer;
                    }
                    break;
            }

            return server;
        }

        public override void DisconnectAll()
        {
            foreach (var photonServerPeer in Servers)
            {
                photonServerPeer.Value.Disconnect();
            }

            foreach (var photonClientPeer in Clients)
            {
                photonClientPeer.Value.Disconnect();
            }
        }
    }
}