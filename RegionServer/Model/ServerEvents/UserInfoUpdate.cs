using AndorServer;
using AndorServerCommon;
using AndorServerCommon.MessageObjects;

namespace RegionServer.Model.ServerEvents
{
    public class UserInfoUpdate : ServerPacket
    {
        public UserInfoUpdate(CPlayerInstance player)
            : base(ClientEventCode.ServerPacket, MessageSubCode.UserInfo)
        {
            AddParameter(player.ObjectId, ClientParameterCode.ObjectId);
            AddUserInfo(player);
        }

        private void AddUserInfo(CPlayerInstance player)
        {
            UserInfo info = new UserInfo
            {
                Position = player.Position,
                Name = player.Name,

                // Attributes, level, exp, stats
                // inventory - all equipped items
                // Talents etc.. skills
                // effects movement speeds
                // actions/emotes
            };

            AddSerializedParameter(info, ClientParameterCode.Object, false);
        }
    }
}