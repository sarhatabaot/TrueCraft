using TrueCraft.Networking;

namespace TrueCraft.Server
{
	/// <summary>
	///  Called when the given packet comes in from a remote client. Return false to cease communication
	///  with that client.
	/// </summary>
	public delegate void PacketHandler(IPacket packet, IRemoteClient client, IMultiPlayerServer server);
}