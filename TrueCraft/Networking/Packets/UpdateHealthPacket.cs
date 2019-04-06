namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by servers to inform clients of their current health.
	/// </summary>
	[MessageTarget(MessageTarget.Client)]
	public struct UpdateHealthPacket : IPacket
	{
		public byte Id => Constants.PacketIds.UpdateHealth;

		public UpdateHealthPacket(short health) => Health = health;

		public short Health;

		public void ReadPacket(IMcStream stream)
		{
			Health = stream.ReadInt16();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt16(Health);
		}
	}
}