namespace TrueCraft.Networking.Packets
{
	[MessageTarget(MessageTarget.Client)]
	public struct SpawnPlayerPacket : IPacket
	{
		public byte Id => Constants.PacketIds.SpawnPlayer;

		public int EntityId;
		public string PlayerName;
		public int X, Y, Z;
		public sbyte Yaw, Pitch;

		/// <summary>
		///  Note that this should be 0 for "no item".
		/// </summary>
		public short CurrentItem;

		public SpawnPlayerPacket(int entityID, string playerName, int x, int y, int z, sbyte yaw, sbyte pitch,
			short currentItem)
		{
			EntityId = entityID;
			PlayerName = playerName;
			X = x;
			Y = y;
			Z = z;
			Yaw = yaw;
			Pitch = pitch;
			CurrentItem = currentItem;
		}

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			PlayerName = stream.ReadString();
			X = stream.ReadInt32();
			Y = stream.ReadInt32();
			Z = stream.ReadInt32();
			Yaw = stream.ReadInt8();
			Pitch = stream.ReadInt8();
			CurrentItem = stream.ReadInt16();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			stream.WriteString(PlayerName);
			stream.WriteInt32(X);
			stream.WriteInt32(Y);
			stream.WriteInt32(Z);
			stream.WriteInt8(Yaw);
			stream.WriteInt8(Pitch);
			stream.WriteInt16(CurrentItem);
		}
	}
}