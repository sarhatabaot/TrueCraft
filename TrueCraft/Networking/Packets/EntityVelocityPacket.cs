namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by servers to inform players of changes to the velocity of entities.
	/// </summary>
	[MessageTarget(MessageTarget.Client)]
	public struct EntityVelocityPacket : IPacket
	{
		public byte Id => Constants.PacketIds.EntityVelocity;

		public int EntityId;
		public short XVelocity;
		public short YVelocity;
		public short ZVelocity;

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			XVelocity = stream.ReadInt16();
			YVelocity = stream.ReadInt16();
			ZVelocity = stream.ReadInt16();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			stream.WriteInt16(XVelocity);
			stream.WriteInt16(YVelocity);
			stream.WriteInt16(ZVelocity);
		}
	}
}