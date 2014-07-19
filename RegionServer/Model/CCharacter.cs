using AndorServerCommon.MessageObjects;
using RegionServer.Model.KnownList;
using RegionServer.Model.Interfaces;
using System.Collections.Generic;
using RegionServer.Model.ServerEvents;

namespace RegionServer.Model
{
    public class CCharacter : CObject, ICharacter
    {
        public CCharacter(Region region, ObjectKnownList objectKnownList, IStatHolder stats)
            : base(region, objectKnownList)
        {
            Stats = stats;
            stats.Character = this;

            StatusListeners = new List<ICharacter>();
        }

        public bool IsTeleporting { get; private set; }
        public bool IsDead { get; private set; }

        public Position Destination { get; set; }
        public virtual MoveDirection Direction { get; set; }
        public virtual bool Moving { get; set; }
        public int Facing { get; set; }

        public IList<ICharacter> StatusListeners { get; private set; }
        public IStatHolder Stats { get; protected set; }

        public delegate void DeathListener(ICharacter killer);
        public DeathListener DeathListeners;

        public new CharacterKnowntList KnownList
        {
            get
            {
                return ObjectKnownList as CharacterKnowntList;
            }
        }

        private IObject _target;
        public IObject Target { get { return _target; }
            set
            {
                if (value != null && !value.IsVisible)
                {
                    value = null;
                }

                if (value != null && value != _target)
                {
                    KnownList.AddKnownObject(value);
                    value.KnownList.AddKnownObject(value);
                }
            }
        }

        public virtual void BroadcastMessage(ServerPacket packet)
        {
            foreach (CPlayerInstance player in KnownList.KnownPlayers.Values)
            {
                player.SendPacket(packet);
            }
        }

        public virtual void SendMessage(string text)
        {
        }

        public void Teleport(Position pos)
        {
            Teleport(pos.X, pos.Y, pos.Z, pos.Heading);
        }

        public void Teleport(float x, float y, float z, short heading)
        {
            StopMove(null);

            IsTeleporting = true;

            Target = null;

            BroadcastMessage(new TeleportToLocation(this, x, y, z, heading));

            Decay();
            Position.XYZ(x, y, z);
            Position.Heading = heading;
        }

        public void Teleport(float x, float y, float z)
        {
            Teleport(x, y, z, Position.Heading);
        }

        public void Teleport(ITeleportType teleportType)
        {
            Teleport(teleportType.GetNearestTeleportLocation(this));
        }

        public virtual bool Die(ICharacter killer)
        {
            if (IsDead)
            {
                return false;
            }

            //Stats.OnDeath();
            IsDead = true;

            if (DeathListeners != null)
            {
                DeathListeners(killer);
            }

            Target = null;
            StopMove(null);

            //Effects.StopAllEffectsThroughDeath();

            CalculateRewards(killer);
            BroadcastStatusUpdate();

            //Region.OnDeath(this);

            return true;
        }

        public int TargetId
        {
            get 
            {
                if (Target != null)
                {
                    return Target.ObjectId;
                }

                return -1;
            }
        }

        public void BroadcastStatusUpdate()
        {
            foreach (var statusListener in StatusListeners)
            {
                statusListener.BroadcastMessage(new StatusUpdate(this));
            }
        }

        public virtual void UpdateAndBroadcastStatus(int broadcastType)
        {
        }

        public virtual void CalculateRewards(ICharacter killer)
        {
        }

        public void StopMove(Position pos)
        {
            if (pos != null)
            {
                Destination = pos;
                Position = pos;
            }
            else
            {
                Destination = Position;
            }

            BroadcastMessage(new StopMove(this));
        }

        public virtual void SendStateToPlayer(IObject owner)
        {
            owner.SendPacket(new MoveToLocation(this));
        }

        public virtual void DeleteMe()
        {
            
        }
    }
}