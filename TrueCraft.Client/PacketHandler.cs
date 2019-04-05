using TrueCraft.Networking;

namespace TrueCraft.Client
{
	public delegate void PacketHandler(IPacket packet, MultiPlayerClient client);
}