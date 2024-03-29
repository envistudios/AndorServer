﻿using System;
using System.Collections.Generic;
using System.Text;

using MMO.Photon.Server;
using MMO.Photon.Application;
using MMO.Framework;
using AndorServerCommon;
using Photon.SocketServer;
using LoginServer.Operations;
using SubServerCommon;
using SubServerCommon.Data.NHibernate;
using System.Security.Cryptography;

namespace LoginServer.Handlers
{
    public class AndorServerRegisterRequestHandler : PhotonServerHandler
    {
        public AndorServerRegisterRequestHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte)ClientOperationCode.Login; }
        }

        public override int? SubCode
        {
            get { return (int)MessageSubCode.Register; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var operation = new RegisterSecurely(serverPeer.Protocol, message);

            if (!operation.IsValid)
            {
                serverPeer.SendOperationResponse(new OperationResponse(message.Code, 
                    new Dictionary<byte, object>{{(byte)ClientParameterCode.PeerId, message.Parameters[(byte)ClientParameterCode.PeerId]}})
                    {
                        ReturnCode = (int)ErrorCode.OperationInvalid,
                        DebugMessage = operation.GetErrorMessage()
                    }, new SendParameters());

                return true;
            }

            if (operation.UserName == "" || operation.Email == "" || operation.Password == "")
            {
                serverPeer.SendOperationResponse(new OperationResponse(message.Code,
                new Dictionary<byte, object> { { (byte)ClientParameterCode.PeerId, message.Parameters[(byte)ClientParameterCode.PeerId]}})
                    {
                        ReturnCode = (int)ErrorCode.OperationInvalid,
                        DebugMessage = "All fields are required"
                    }, new SendParameters());

                return true;
            }

            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        Log.Debug("About to look for user account");
                        var userList = session.QueryOver<User>().Where(u => u.Username == operation.UserName).List();

                        if (userList.Count > 0)
                        {
                            Log.DebugFormat("Found account name already in use");
                            transaction.Commit();
                            serverPeer.SendOperationResponse(new OperationResponse(message.Code)
                                {
                                    ReturnCode = (int)ErrorCode.UserNameInUse,
                                    DebugMessage = "Account name already in use, please use another"
                                }, new SendParameters());

                            return true;
                        }

                        string salt = Guid.NewGuid().ToString().Replace("-", "");
                        Log.DebugFormat("Created salt {0}", salt);
                        User newUser = new User()
                        {
                            Email = operation.Email,
                            Username = operation.UserName,
                            Password = 
                                BitConverter.ToString(SHA1CryptoServiceProvider.Create().ComputeHash(
                                    Encoding.UTF8.GetBytes(salt + operation.Password))).Replace("-", ""),
                            Salt = salt,
                            Algorithm = "sha1",
                            Created = DateTime.Now,
                            Updated = DateTime.Now
                        };

                        Log.DebugFormat("Built user object");
                        session.Save(newUser);
                        Log.DebugFormat("Saved new user");
                        transaction.Commit();
                    }

                    using (var transaction = session.BeginTransaction())
                    {
                        Log.DebugFormat("Looking up newly created user");
                        var userList = session.QueryOver<User>().Where(u => u.Username == operation.UserName).List();

                        if (userList.Count > 0)
                        {
                            Log.DebugFormat("Creating Profile");
                            UserProfile profile = new UserProfile() { CharacterSlots = 1, UserId = userList[0] };
                            session.Save(profile);
                            Log.DebugFormat("Saved Profile");
                            transaction.Commit();
                        }
                    }

                    serverPeer.SendOperationResponse(
                        new OperationResponse(message.Code) { ReturnCode = (byte)ClientReturnCode.UserCreated}, 
                        new SendParameters());
                }
            }
            catch (Exception e)
            {
                Log.Error("Error Occured", e);
                serverPeer.SendOperationResponse(new OperationResponse(message.Code,
                    new Dictionary<byte, object> { { (byte)ClientParameterCode.PeerId, message.Parameters[(byte)ClientParameterCode.PeerId] } })
                    {
                        ReturnCode = (int)ErrorCode.UserNameInUse,
                        DebugMessage = e.ToString()
                    }, new SendParameters());
            }

            return true;
        }
    }
}