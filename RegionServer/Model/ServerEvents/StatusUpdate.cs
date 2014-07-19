using AndorServer;
using AndorServerCommon;

namespace RegionServer.Model.ServerEvents
{
    public class StatusUpdate : ServerPacket
    {
        public StatusUpdate(CCharacter character)
            : base(ClientEventCode.ServerPacket, AndorServerCommon.MessageSubCode.StatusUpdate)
        {
            AddParameter(character.ObjectId, ClientParameterCode.ObjectId);
        }
    }
}