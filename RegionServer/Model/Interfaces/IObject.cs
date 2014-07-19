﻿using AndorServerCommon;
using RegionServer.Model.KnownList;
using RegionServer.Model.ServerEvents;

namespace RegionServer.Model.Interfaces
{
    public interface IObject
    {
        int InstanceId { get; set; }
        int ObjectId { get; }
        bool IsVisible { get; set; }
        string Name { get; set; }
        Position Position { get; set; }
        ObjectKnownList KnownList { get; set; }
        Region Region { get; }

        void Spawn();
        void Decay();

        void SendPacket(ServerPacket packet);
        void SendPacket(SystemMessageId id);
        void SendInfo(IObject obj);
        void OnSpawn();
        void ToggleVisible();
    }
}