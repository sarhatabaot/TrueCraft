namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by the server to allow the player to spawn, with information about the world being spawned into.
	/// </summary>
	[MessageTarget(MessageTarget.Client)]
	public struct LoginResponsePacket : IPacket
	{
		public byte Id => Constants.PacketIds.LoginResponse;

		public LoginResponsePacket(int entityID, long seed, Dimension dimension)
		{
			EntityId = entityID;
			Seed = seed;
			Dimension = dimension;
		}

		public int EntityId;
		public long Seed;
		public Dimension Dimension;

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			stream.ReadString(); // Unused
			Seed = stream.ReadInt64();
			Dimension = (Dimension) stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			stream.WriteString(""); // Unused
			stream.WriteInt64(Seed);
			stream.WriteInt8((sbyte) Dimension);
		}
	}
}