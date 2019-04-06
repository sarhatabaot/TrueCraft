namespace TrueCraft.Networking.Packets
{
	[MessageTarget(MessageTarget.Client)]
	public struct SpawnMobPacket : IPacket
	{
		public byte Id => Constants.PacketIds.SpawnMob;

		public SpawnMobPacket(int entityId, sbyte type, int x, int y, int z, sbyte yaw, sbyte pitch,
			MetadataDictionary metadata)
		{
			EntityId = entityId;
			MobType = type;
			X = x;
			Y = y;
			Z = z;
			Yaw = yaw;
			Pitch = pitch;
			Metadata = metadata;
		}

		public int EntityId;
		public sbyte MobType;
		public int X, Y, Z;
		public sbyte Yaw, Pitch;
		public MetadataDictionary Metadata;

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			MobType = stream.ReadInt8();
			X = stream.ReadInt32();
			Y = stream.ReadInt32();
			Z = stream.ReadInt32();
			Yaw = stream.ReadInt8();
			Pitch = stream.ReadInt8();
			Metadata = MetadataDictionary.FromStream(stream);
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			stream.WriteInt8(MobType);
			stream.WriteInt32(X);
			stream.WriteInt32(Y);
			stream.WriteInt32(Z);
			stream.WriteInt8(Yaw);
			stream.WriteInt8(Pitch);
			Metadata.WriteTo(stream);
		}
	}
}