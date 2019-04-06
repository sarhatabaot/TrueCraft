namespace TrueCraft.Networking.Packets
{
	[MessageTarget(MessageTarget.Client)]
	public struct BulkBlockChangePacket : IPacket
	{
		public byte Id => Constants.PacketIds.BulkBlockChange;

		public int ChunkX, ChunkZ;
		public Coordinates3D[] Coordinates;
		public sbyte[] BlockIDs;
		public sbyte[] Metadata;

		public void ReadPacket(IMcStream stream)
		{
			ChunkX = stream.ReadInt32();
			ChunkZ = stream.ReadInt32();
			var length = stream.ReadInt16();
			Coordinates = new Coordinates3D[length];
			for (var i = 0; i < length; i++)
			{
				var value = stream.ReadUInt16();
				Coordinates[i] = new Coordinates3D(
					(value >> 12) & 0xF,
					value & 0xFF,
					(value >> 8) & 0xF);
			}

			BlockIDs = stream.ReadInt8Array(length);
			Metadata = stream.ReadInt8Array(length);
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(ChunkX);
			stream.WriteInt32(ChunkZ);
			stream.WriteInt16((short) Coordinates.Length);
			for (var i = 0; i < Coordinates.Length; i++)
			{
				var coord = Coordinates[i];
				stream.WriteUInt16((ushort) (((coord.X << 12) & 0xF) | ((coord.Z << 8) & 0xF) | (coord.Y & 0xFF)));
			}

			stream.WriteInt8Array(BlockIDs);
			stream.WriteInt8Array(Metadata);
		}
	}
}