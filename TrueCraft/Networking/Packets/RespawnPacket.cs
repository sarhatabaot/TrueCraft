namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by clients when the player clicks "Respawn" after death. Sent by servers to confirm
	///  the respawn, and to respawn players in different dimensions (i.e. when using a portal).
	/// </summary>
	public struct RespawnPacket : IPacket
	{
		public byte Id => Constants.PacketIds.Respawn;

		public Dimension Dimension;

		public void ReadPacket(IMcStream stream)
		{
			Dimension = (Dimension) stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt8((sbyte) Dimension);
		}
	}
}