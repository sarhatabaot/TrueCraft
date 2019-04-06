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
			EntityId = entityID;
			Slot = slot;
			ItemID = itemID;
			Metadata = metadata;
		}

		public int EntityId;
		public short Slot;

		/// <summary>
		///  The Id of the item to show on this player. Set to -1 for nothing.
		/// </summary>
		public short ItemID;

		public short Metadata;

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			Slot = stream.ReadInt16();
			ItemID = stream.ReadInt16();
			Metadata = stream.ReadInt16();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			stream.WriteInt16(Slot);
			stream.WriteInt16(ItemID);
			stream.WriteInt16(Metadata);
		}
	}
}