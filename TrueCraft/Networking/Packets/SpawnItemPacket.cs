namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by servers to spawn item entities.
	/// </summary>
	[MessageTarget(MessageTarget.Client)]
	public struct SpawnItemPacket : IPacket
	{
		public byte Id => Constants.PacketIds.SpawnItem;

		public int EntityId;
		public short ItemId;
		public sbyte Count;
		public short Metadata;
		public int X, Y, Z;
		public sbyte Yaw;
		public sbyte Pitch;
		public sbyte Roll;

		public SpawnItemPacket(int entityID, short itemID, sbyte count, short metadata, int x, int y, int z, sbyte yaw,
			sbyte pitch, sbyte roll)
		{
			EntityId = entityID;
			ItemId = itemID;
			Count = count;
			Metadata = metadata;
			X = x;
			Y = y;
			Z = z;
			Yaw = yaw;
			Pitch = pitch;
			Roll = roll;
		}

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			ItemId = stream.ReadInt16();
			Count = stream.ReadInt8();
			Metadata = stream.ReadInt16();
			X = stream.ReadInt32();
			Y = stream.ReadInt32();
			Z = stream.ReadInt32();
			Yaw = stream.ReadInt8();
			Pitch = stream.ReadInt8();
			Roll = stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			stream.WriteInt16(ItemId);
			stream.WriteInt8(Count);
			stream.WriteInt16(Metadata);
			stream.WriteInt32(X);
			stream.WriteInt32(Y);
			stream.WriteInt32(Z);
			stream.WriteInt8(Yaw);
			stream.WriteInt8(Pitch);
			stream.WriteInt8(Roll);
		}
	}
}