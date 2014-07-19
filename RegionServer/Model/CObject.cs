using RegionServer.Model.Interfaces;
using RegionServer.Model.KnownList;
using AndorServerCommon;
using RegionServer.Model.ServerEvents;

namespace RegionServer.Model
{
    public class CObject : IObject
    {
        protected IKnownList ObjectKnownList;

        public virtual ObjectKnownList KnownList 
        { 
            get 
            { 
                return ObjectKnownList as ObjectKnownList;
            }
            set
            {
                ObjectKnownList = value;
            }
        }

        public CObject(Region region, ObjectKnownList objectKnownList)
        {
            _region = region;
            ObjectKnownList = objectKnownList;
            ObjectKnownList.Owner = this;
            //Position = new Position();
        }

        private readonly Region _region;
        public Region Region { get { return _region; } }

        public int InstanceId { get; set; }
        public int ObjectId { get; set; }
        public bool IsVisible { get; set; }
        public string Name { get; set; }
        public virtual Position Position { get; set; }

        public void Spawn()
        {
            IsVisible = true;

            // Region Code
            Region.AddObject(this);
            Region.AddVisibleObject(this);

            OnSpawn();
        }

        public void Decay()
        {
            IsVisible = false;

            // Region Code
            Region.RemoveVisibleObject(this);
            Region.RemoveObject(this);
        }

        public virtual void SendPacket(ServerPacket packet)
        {
        }

        public virtual void SendPacket(SystemMessageId id)
        {
        }

        public virtual void SendInfo(IObject obj)
        {
        }

        public virtual void OnSpawn()
        {
        }

        public void ToggleVisible()
        {
            if (IsVisible)
            {
                Decay();
            }
            else
            {
                Spawn();
            }
        }
    }
}