using System;
using System.Linq;
using AndorServerCommon.MessageObjects;
using RegionServer.Model.Interfaces;
using MMO.Photon.Client;
using MMO.Photon.Server;
using RegionServer.Model.ServerEvents;
using AndorServerCommon;
using Photon.SocketServer;
using RegionServer.Model.KnownList;
using MMO.Framework;
using RegionServer.Model.Stats;
using SubServerCommon;
using SubServerCommon.Data.NHibernate;

namespace RegionServer.Model
{
    public class CPlayerInstance : CPlayable, IPlayer, IClientData
    {
        public CPlayerInstance(Region region, PlayerKnownList objectKnownList,  IStatHolder stats, IPhysics physics)
            : base(region, objectKnownList, stats)
        {
            Physics = physics;
            Physics.MoveSpeed = Stats.GetStat<MoveSpeed>();
            Destination = new Position();
        }

        public SubServerClientPeer Client { get; set; }
        public PhotonServerPeer ServerPeer { get; set; }
        public int? UserId { get; set; }
        public int? CharacterId { get; set; }
        public IPhysics Physics { get; set; }

        public override Position Position
        {
            get
            {
                base.Position = Physics.Position;
                return base.Position;
            }
            set
            {
                base.Position = value;

                if (Physics != null)
                {
                    Physics.Position = value;
                }
            }

        }

        public override MoveDirection Direction
        {
            get
            {
                base.Direction = Physics.Direction;
                return base.Direction;
            }
            set
            {
                base.Direction = value;
            }
        }

        public override bool Moving
        {
            get
            {
                base.Moving = Physics.Moving;
                return base.Moving;
            }
            set
            {
                base.Moving = value;
            }
        }

        public new PlayerKnownList KnownList
        {
            get
            {
                return ObjectKnownList as PlayerKnownList;
            }
            set
            {
                ObjectKnownList = value;
            }
        }

        public override void BroadcastMessage(ServerPacket packet)
        {
            if (packet.SendToSelf)
            {
                SendPacket(packet);
            }

            foreach (CPlayerInstance player in KnownList.KnownPlayers.Values)
            {
                player.SendPacket(packet);
            }
        }

        public override void SendPacket(ServerPacket packet)
        {
            if (Client != null)
            {
                Client.Log.DebugFormat("Sending {0} to {1}", packet.GetType(), Name);
                packet.AddParameter(Client.PeerId.ToByteArray(), ClientParameterCode.PeerId);   // Check this..
                ServerPeer.SendEvent(new EventData(packet.Code, packet.Parameters), new SendParameters());
            }
        }

        public override void SendInfo(IObject obj)
        {
            obj.SendPacket(new CharInfoUpdate(this));
        }

        public void BroadcastUserInfo()
        {
            SendPacket(new UserInfoUpdate(this));
            BroadcastMessage(new CharInfoUpdate(this));
        }

        public override void DeleteMe()
        {
            CleanUp();
            Store();

            base.DeleteMe();
        }

        public void CleanUp()
        {
            Client.Log.DebugFormat("Logging Off");

            // abort auto attack or casting
            StopMove(null);

            // rmeove temporary items

            // remove from LFG/Battlegrounds

            // Stop all timers

            // Stop crafting

            Target = null;

            // Stop temp effects

            // Decay from server
            Decay();

            // Remove from all groups

            // unsummon pets

            // notify guild of log off

            // cancle trading

            // notify friend list of log off

        }

        public void Store()
        {
            // Save character to database
            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var user = session.QueryOver<User>().Where(u => u.Id == UserId).SingleOrDefault();
                        var character =
                            session.QueryOver<AndorServerCharacter>().Where(cc => cc.Name == Name).SingleOrDefault();

                        character.Level = (int) Stats.GetStat<Level>();

                        string position = Position.Serialize();
                        character.Position = position;

                        // Store stats
                        character.Stats = Stats.SerializedStats();

                        session.Save(character);

                        transaction.Commit();
                    }
                }
            }
            catch (Exception e)
            {
                Client.Log.Error(e);
            }
        }

        public void Restore(int objectId)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var character =
                        session.QueryOver<AndorServerCharacter>().Where(cc => cc.Id == objectId).List().FirstOrDefault();

                    if (character != null)
                    {
                        Client.Log.DebugFormat("User Restored from Database");
                        transaction.Commit();
                        ObjectId = objectId;
                        Name = character.Name;

                        // Appearance

                        // Level
                        Stats.SetStat<Level>(character.Level);

                        // Exp

                        // Position
                        if (!string.IsNullOrEmpty(character.Position))
                        {
                            Position = Position.Deserialize(character.Position);
                        }
                        else
                        {                          
                            Position = new Position(135f, 6.5f, 165f);
                        }

                        // Guild

                        // Titles

                        // Stats
                        if (!string.IsNullOrEmpty(character.Stats))
                        {
                            Stats.DeserializeStats(character.Stats);
                        }

                        // Equipment

                        // Inventory

                        // effects

                        // social - guild notify, groups notify

                        Client.Log.DebugFormat("Max HP: {0}", Stats.GetStat<Level5HP>());
                    }
                    else
                    {
                        Client.Log.FatalFormat("CPlayerInstance - character is null");
                    }
                }
            }
        }
    }
}