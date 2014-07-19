using AndorServer;
using AndorServerCommon;
using AndorServerCommon.MessageObjects;

namespace RegionServer.Model.ServerEvents
{
    public class CharInfoUpdate : ServerPacket
    {
        public CharInfoUpdate(CPlayerInstance player)
            : base(ClientEventCode.ServerPacket, MessageSubCode.CharInfo, false)
        {       
            AddParameter(player.ObjectId, ClientParameterCode.ObjectId);
            AddCharInfo(player);
        }

        private void AddCharInfo(CPlayerInstance player)
        {
            CharInfo info = new CharInfo()
            {
                Position = player.Position,
                Name = player.Name

                // Race, Gender, Class, Title
                // Guild, Inventory, Armor etc..

                // effects - pvp flag, debufs, buffs

                // movement speed for smoothing/calc

                // action/emote walk, run, sit
            };

            AddSerializedParameter(info, ClientParameterCode.Object, false);
        }
    }
}