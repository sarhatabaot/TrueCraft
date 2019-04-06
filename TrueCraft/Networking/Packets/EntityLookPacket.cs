namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by servers to update the direction an entity is looking in.
	/// </summary>
	public struct EntityLookPacket : IPacket
	{
		public byte Id => Constants.PacketIds.EntityLook;

		public int EntityID;
		public sbyte Yaw, Pitch;

		public void ReadPacket(IMcStream stream)
		{
			EntityID = stream.ReadInt32();
			Yaw = stream.ReadInt8();
			Pitch = stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityID);
			stream.WriteInt8(Yaw);
			stream.WriteInt8(Pitch);
		}
	}
}