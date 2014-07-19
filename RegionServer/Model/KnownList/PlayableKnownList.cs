using System.Threading.Tasks;

using RegionServer.Model.Interfaces;

namespace RegionServer.Model.KnownList
{
    public class PlayableKnownList : CharacterKnowntList
    {
        public override void FindObjects()
        {
            Parallel.ForEach(Owner.Region.VisibleObjects, obj =>
                                                            {
                                                                if (obj != Owner)
                                                                {
                                                                    AddKnownObject(obj);

                                                                    if (obj is ICharacter)
                                                                    {
                                                                        obj.KnownList.AddKnownObject(Owner);
                                                                    }
                                                                }
                                                            });
        }
    }
}