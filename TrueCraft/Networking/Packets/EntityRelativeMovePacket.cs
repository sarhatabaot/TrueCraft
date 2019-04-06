namespace TrueCraft.Networking.Packets
{
	[MessageTarget(MessageTarget.Client)]
	public struct EntityRelativeMovePacket : IPacket
	{
		public byte Id => Constants.PacketIds.EntityRelativeMove;

		public int EntityId;
		public sbyte DeltaX, DeltaY, DeltaZ;

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			DeltaX = stream.ReadInt8();
			DeltaY = stream.ReadInt8();
			DeltaZ = stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			stream.WriteInt8(DeltaX);
			stream.WriteInt8(DeltaY);
			stream.WriteInt8(DeltaZ);
		}
	}
}