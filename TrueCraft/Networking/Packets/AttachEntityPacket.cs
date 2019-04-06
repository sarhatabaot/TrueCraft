namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Used to attach entities to other entities, i.e. players to carts
	/// </summary>
	[MessageTarget(MessageTarget.Client)]
	public struct AttachEntityPacket : IPacket
	{
		public byte Id => Constants.PacketIds.AttachEntity;

		public int EntityId;
		public int VehicleID;

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			VehicleID = stream.ReadInt32();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			stream.WriteInt32(VehicleID);
		}
	}
}