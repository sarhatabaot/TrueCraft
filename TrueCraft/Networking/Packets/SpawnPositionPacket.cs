namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by the server to specify the coordinates of the spawn point. This only affects what the
	///  compass item points to.
	/// </summary>
	public struct SpawnPositionPacket : IPacket
	{
		public byte ID => 0x06;

		public SpawnPositionPacket(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public int X, Y, Z;

		public void ReadPacket(IMcStream stream)
		{
			X = stream.ReadInt32();
			Y = stream.ReadInt32();
			Z = stream.ReadInt32();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(X);
			stream.WriteInt32(Y);
			stream.WriteInt32(Z);
		}
	}
}