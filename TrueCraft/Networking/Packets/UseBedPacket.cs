namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by servers to indicate that a player is using a bed. More research required on this packet.
	/// </summary>
	[MessageTarget(MessageTarget.Client)]
	public struct UseBedPacket : IPacket
	{
		public byte Id => Constants.PacketIds.UseBed;

		public int EntityId;
		public bool InBed;
		public int X;
		public sbyte Y;
		public int Z;

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			InBed = stream.ReadBoolean();
			X = stream.ReadInt32();
			Y = stream.ReadInt8();
			Z = stream.ReadInt32();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			stream.WriteBoolean(InBed);
			stream.WriteInt32(X);
			stream.WriteInt8(Y);
			stream.WriteInt32(Z);
		}
	}
}