namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Spawns lightning at the given coordinates.
	/// </summary>
	[MessageTarget(MessageTarget.Client)]
	public struct LightningPacket : IPacket
	{
		public byte Id => Constants.PacketIds.Lightning;

		public int EntityId;
		public int X, Y, Z;

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			stream.ReadBoolean(); // Unknown
			X = stream.ReadInt32();
			Y = stream.ReadInt32();
			Z = stream.ReadInt32();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			stream.WriteBoolean(true); // Unknown
			stream.WriteInt32(X);
			stream.WriteInt32(Y);
			stream.WriteInt32(Z);
		}
	}
}