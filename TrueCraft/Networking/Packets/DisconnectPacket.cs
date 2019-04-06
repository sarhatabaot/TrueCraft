namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Disconnects from a server or kicks a player. This is the last packet sent.
	/// </summary>
	public struct DisconnectPacket : IPacket
	{
		public byte Id => Constants.PacketIds.Disconnect;

		public DisconnectPacket(string reason) => Reason = reason;

		public string Reason;

		public void ReadPacket(IMcStream stream)
		{
			Reason = stream.ReadString();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteString(Reason);
		}
	}
}