namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent from servers to continue with a connection. A kick is sent instead if the connection is refused.
	/// </summary>
	[MessageTarget(MessageTarget.Client)]
	public struct HandshakeResponsePacket : IPacket
	{
		public byte Id => Constants.PacketIds.HandshakeResponse;

		public HandshakeResponsePacket(string connectionHash) => ConnectionHash = connectionHash;

		/// <summary>
		///  Set to "-" for offline mode servers. Online mode beta servers are obsolete.
		/// </summary>
		public string ConnectionHash;

		public void ReadPacket(IMcStream stream)
		{
			ConnectionHash = stream.ReadString();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteString(ConnectionHash);
		}
	}
}