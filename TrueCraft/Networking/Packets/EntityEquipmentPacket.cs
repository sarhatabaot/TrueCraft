namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sets the equipment visible on player entities (i.e. armor).
	/// </summary>
	[MessageTarget(MessageTarget.Client)]
	public struct EntityEquipmentPacket : IPacket
	{
		public byte Id => Constants.PacketIds.EntityEquipment;

		public EntityEquipmentPacket(int entityID, short slot, short itemID, short metadata)
		{
			EntityID = entityID;
			Slot = slot;
			ItemID = itemID;
			Metadata = metadata;
		}

		public int EntityID;
		public short Slot;

		/// <summary>
		///  The ID of the item to show on this player. Set to -1 for nothing.
		/// </summary>
		public short ItemID;

		public short Metadata;

		public void ReadPacket(IMcStream stream)
		{
			EntityID = stream.ReadInt32();
			Slot = stream.ReadInt16();
			ItemID = stream.ReadInt16();
			Metadata = stream.ReadInt16();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityID);
			stream.WriteInt16(Slot);
			stream.WriteInt16(ItemID);
			stream.WriteInt16(Metadata);
		}
	}
}