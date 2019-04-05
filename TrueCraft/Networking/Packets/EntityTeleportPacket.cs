namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Used to teleport entities to arbitrary locations.
	/// </summary>
	public struct EntityTeleportPacket : IPacket
	{
		public byte ID => Constants.PacketIds.EntityTeleport;

		public int EntityID;
		public int X, Y, Z;
		public sbyte Yaw, Pitch;

		public EntityTeleportPacket(int entityId, int x, int y, int z, sbyte yaw, sbyte pitch)
		{
			EntityID = entityId;
			X = x;
			Y = y;
			Z = z;
			Yaw = yaw;
			Pitch = pitch;
		}

		public void ReadPacket(IMcStream stream)
		{
			EntityID = stream.ReadInt32();
			X = stream.ReadInt32();
			Y = stream.ReadInt32();
			Z = stream.ReadInt32();
			Yaw = stream.ReadInt8();
			Pitch = stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityID);
			stream.WriteInt32(X);
			stream.WriteInt32(Y);
			stream.WriteInt32(Z);
			stream.WriteInt8(Yaw);
			stream.WriteInt8(Pitch);
		}
	}
}