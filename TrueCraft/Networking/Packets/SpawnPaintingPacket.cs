namespace TrueCraft.Networking.Packets
{
	[MessageTarget(MessageTarget.Client)]
	public struct SpawnPaintingPacket : IPacket
	{
		public enum PaintingDirection
		{
			NegativeZ = 0,
			NegativeX = 1,
			PositiveZ = 2,
			PositiveX = 3
		}

		public byte Id => Constants.PacketIds.SpawnPainting;

		public int EntityId;
		public string PaintingName;
		public int X, Y, Z;
		public PaintingDirection Direction;

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			PaintingName = stream.ReadString();
			X = stream.ReadInt32();
			Y = stream.ReadInt32();
			Z = stream.ReadInt32();
			Direction = (PaintingDirection) stream.ReadInt32();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			stream.WriteString(PaintingName);
			stream.WriteInt32(X);
			stream.WriteInt32(Y);
			stream.WriteInt32(Z);
			stream.WriteInt32((int) Direction);
		}
	}
}