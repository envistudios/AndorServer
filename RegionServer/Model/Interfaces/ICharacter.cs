using System.Collections.Generic;
using AndorServerCommon.MessageObjects;
using RegionServer.Model.ServerEvents;

namespace RegionServer.Model.Interfaces
{
    public interface ICharacter
    {
        IObject Target { get; set; }
        int TargetId { get; }
        bool IsTeleporting { get; }
        bool IsDead { get; }
        Position Destination { get; set; }
        MoveDirection Direction { get; set; }

        IList<ICharacter> StatusListeners { get; }
        IStatHolder Stats { get; }
        bool Moving { get; set; }

        void BroadcastMessage(ServerPacket packet);
        void BroadcastStatusUpdate();
        void UpdateAndBroadcastStatus(int broadcastType);
        void SendMessage(string text);

        void Teleport(Position pos);
        void Teleport(float x, float y, float z, short heading);
        void Teleport(float x, float y, float z);
        void Teleport(ITeleportType teleportType);

        bool Die(ICharacter killer);
        void CalculateRewards(ICharacter killer);

        void StopMove(Position pos);

        void SendStateToPlayer(IObject owner);
    }
}