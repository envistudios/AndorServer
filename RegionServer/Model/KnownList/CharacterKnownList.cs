using System.Collections.Concurrent;
using System.Collections.Generic;

using RegionServer.Model.Interfaces;
using System.Threading.Tasks;
using System.Linq;

namespace RegionServer.Model.KnownList
{
    public class CharacterKnowntList : ObjectKnownList
    {
        public ConcurrentDictionary<int, IPlayer> KnownPlayers { get; set; }

        public CharacterKnowntList()
            : base()
        {
            KnownPlayers = new ConcurrentDictionary<int, IPlayer>();
        }

        public override bool AddKnownObject(IObject obj)
        {
            if (!base.AddKnownObject(obj))
            {
                return false;
            }

            IPlayer player = obj as IPlayer;

            if (player != null)
            {
                KnownPlayers.TryAdd(obj.ObjectId, player);
            }

            return true;
        }

        public override void FindObjects()
        {
            Parallel.ForEach(Owner.Region.VisibleObjects, obj =>
                                                            {
                                                                if (obj != Owner)
                                                                {
                                                                    AddKnownObject(obj);
                                                                }
                                                            });
        }

        public override void RemoveAllKnownObjects()
        {
 	        base.RemoveAllKnownObjects();

            KnownPlayers.Clear();
        }

        public override bool RemoveKnownObject(IObject obj)
        {
            if (!base.RemoveKnownObject(obj))
            {
                return false;
            }

            IPlayer player = obj as IPlayer;

            if (player != null)
            {
                KnownPlayers.TryRemove(obj.ObjectId, out player);
            }

            return true;
        }

        public override void ForgetObjects(bool fullCheck)
        {
            if (!fullCheck)
            {
                var players = KnownPlayers.Values;

                foreach (IObject player in players)
                {
                    if (!player.IsVisible || !Util.IsInShortRange(DistanceToForgetObject(player), Owner, player, true))
                    {
                        RemoveKnownObject(player);
                    }
                }

                return;
            }

            var objs = KnownObjects.Values;

            foreach (var obj in objs)
            {
                if (!obj.IsVisible || !Util.IsInShortRange(DistanceToForgetObject(obj), Owner, obj, true))
                {
                    RemoveKnownObject(obj);

                    IPlayer player;

                    if (obj is IPlayer)
                    {
                        KnownPlayers.TryRemove(obj.ObjectId, out player);
                    }
                }
            }
        }

        public List<ICharacter> KnownCharacters
        {
            get { return KnownObjects.Values.Where(obj => obj is ICharacter).Cast<ICharacter>().ToList(); }
        }

        public List<ICharacter> KnownCharactersInRadius(int radius)
        {
            return KnownObjects.Values.Where(obj => obj is ICharacter && Util.IsInShortRange(radius, Owner, obj, true)).Cast<ICharacter>().ToList();
        }

        public List<IPlayer> KnownPlayersInRadius(int radius)
        {
            return KnownPlayers.Values.Where(obj => obj is IObject && Util.IsInShortRange(radius, Owner, (IObject)obj, true)).ToList();
        }
    }
}