using TrueCraft.API.Networking;

namespace TrueCraft.Client
{
	public delegate void PacketHandler(IPacket packet, MultiplayerClient client);
}