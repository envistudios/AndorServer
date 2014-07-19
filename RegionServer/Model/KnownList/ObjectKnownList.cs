using System.Collections.Concurrent;
using RegionServer.Model.Interfaces;
using System.Linq;

namespace RegionServer.Model.KnownList
{
    public class ObjectKnownList : IKnownList
    {
        protected ConcurrentDictionary<int, IObject> KnownObjects;

        public IObject Owner{ get; set; }

        public ObjectKnownList()
        {
            KnownObjects = new ConcurrentDictionary<int, IObject>();
        }

        public virtual bool AddKnownObject(IObject obj)
        {
            if (obj == null)
            {
                return false;
            }

            // if owner.InstanceId == -1 - Assume it is a GM
            if (Owner.InstanceId != -1 && obj.InstanceId != Owner.InstanceId)
            {
                return false;
            }

            if (KnowsObject(obj))
            {
                return false;
            }

            if (!Util.IsInShortRange(DistanceToWatchObject(obj), Owner, obj, true))
            {
                return false;
            }

            KnownObjects.TryAdd(obj.ObjectId, obj);

            return true;
        }

        public bool KnowsObject(IObject obj)
        {
            return Owner == obj || KnownObjects.ContainsKey(obj.ObjectId);
        }

        public virtual void RemoveAllKnownObjects()
        {
            KnownObjects.Clear();
        }

        public virtual bool RemoveKnownObject(IObject obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (KnowsObject(obj))
            {
                KnownObjects.TryRemove(obj.ObjectId, out obj);

                return true;
            }

            return false;
        }

        public virtual void FindObjects()
        {            
        }

        public virtual void ForgetObjects(bool fullCheck)
        {
            var values = KnownObjects.Values.ToList();
            var iter = values.GetEnumerator();

            while (iter.MoveNext())
            {
                if (iter.Current == null)
                {
                    values.Remove(iter.Current);

                    continue;
                }

                if (!fullCheck && !(iter.Current is IPlayable))
                {
                    continue;
                }

                if (!iter.Current.IsVisible || !Util.IsInShortRange(DistanceToForgetObject(iter.Current), 
                    Owner, iter.Current, true))
                {
                    values.Remove(iter.Current);
                }
            }

            ConcurrentDictionary<int, IObject> newKnownObjects = new ConcurrentDictionary<int, IObject>();

            foreach (var value in values)
            {
                newKnownObjects.TryAdd(value.ObjectId, value);
            }

            KnownObjects = newKnownObjects;
        }

        public virtual int DistanceToForgetObject(IObject obj)
        {
            return 0;
        }

        public virtual int DistanceToWatchObject(IObject obj)
        {
            return 0;
        }
    }
}