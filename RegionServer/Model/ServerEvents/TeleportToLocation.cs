using AndorServer;
using AndorServerCommon;

namespace RegionServer.Model.ServerEvents
{
    public class TeleportToLocation : ServerPacket
    {
        public TeleportToLocation(CObject obj, float x, float y, float z, short heading)
            : this(obj, new Position(x, y, z, heading))
        {
        }

        public TeleportToLocation(CObject obj, Position pos)
            : base(ClientEventCode.ServerPacket, MessageSubCode.TeleportToLocation)
        {
            AddParameter(obj.ObjectId, ClientParameterCode.ObjectId);
            AddSerializedParameter(pos, ClientParameterCode.Object);
        }
    }
}