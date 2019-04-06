namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Used to allocate or unload chunks.
	/// </summary>
	public struct ChunkPreamblePacket : IPacket
	{
		public byte Id => Constants.PacketIds.ChunkPreamble;

		public ChunkPreamblePacket(int x, int z, bool load = true)
		{
			X = x;
			Z = z;
			Load = load;
		}

		public int X, Z;

		/// <summary>
		///  If false, free the chunk. If true, allocate it.
		/// </summary>
		public bool Load;

		public void ReadPacket(IMcStream stream)
		{
			X = stream.ReadInt32();
			Z = stream.ReadInt32();
			Load = stream.ReadBoolean();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(X);
			stream.WriteInt32(Z);
			stream.WriteBoolean(Load);
		}
	}
}