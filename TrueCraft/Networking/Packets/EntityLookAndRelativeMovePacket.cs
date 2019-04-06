namespace TrueCraft.Networking.Packets
{
	[MessageTarget(MessageTarget.Client)]
	public struct EntityLookAndRelativeMovePacket : IPacket
	{
		public byte Id => Constants.PacketIds.EntityLookAndRelativeMove;

		public int EntityId;
		public sbyte DeltaX, DeltaY, DeltaZ;
		public sbyte Yaw, Pitch;

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			DeltaX = stream.ReadInt8();
			DeltaY = stream.ReadInt8();
			DeltaZ = stream.ReadInt8();
			Yaw = stream.ReadInt8();
			Pitch = stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			stream.WriteInt8(DeltaX);
			stream.WriteInt8(DeltaY);
			stream.WriteInt8(DeltaZ);
			stream.WriteInt8(Yaw);
			stream.WriteInt8(Pitch);
		}
	}
}