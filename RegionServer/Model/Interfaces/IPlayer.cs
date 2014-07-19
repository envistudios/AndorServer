using MMO.Photon.Client;
using MMO.Photon.Server;

namespace RegionServer.Model.Interfaces
{
    public interface IPlayer
    {
        SubServerClientPeer Client { get; set; }
        PhotonServerPeer ServerPeer { get; set; }
        int? UserId { get; set; }
        int? CharacterId { get; set; }
        IPhysics Physics { get; set; }
    }
}