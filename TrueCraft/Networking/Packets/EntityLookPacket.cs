namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by servers to update the direction an entity is looking in.
	/// </summary>
	[MessageTarget(MessageTarget.Client)]
	public struct EntityLookPacket : IPacket
	{
		public byte Id => Constants.PacketIds.EntityLook;

		public int EntityId;
		public sbyte Yaw, Pitch;

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			Yaw = stream.ReadInt8();
			Pitch = stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			stream.WriteInt8(Yaw);
			stream.WriteInt8(Pitch);
		}
	}
}