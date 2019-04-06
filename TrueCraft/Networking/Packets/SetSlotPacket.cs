namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sets the contents of an item slot on an inventory window.
	/// </summary>
	[MessageTarget(MessageTarget.Client)]
	public struct SetSlotPacket : IPacket
	{
		public byte Id => Constants.PacketIds.SetSlot;

		public SetSlotPacket(sbyte windowID, short slotIndex, short itemID, sbyte count, short metadata)
		{
			WindowID = windowID;
			SlotIndex = slotIndex;
			ItemId = itemID;
			Count = count;
			Metadata = metadata;
		}

		public sbyte WindowID;
		public short SlotIndex;
		public short ItemId;
		public sbyte Count;
		public short Metadata;

		public void ReadPacket(IMcStream stream)
		{
			WindowID = stream.ReadInt8();
			SlotIndex = stream.ReadInt16();
			ItemId = stream.ReadInt16();
			if (ItemId != -1)
			{
				Count = stream.ReadInt8();
				Metadata = stream.ReadInt16();
			}
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt8(WindowID);
			stream.WriteInt16(SlotIndex);
			stream.WriteInt16(ItemId);
			if (ItemId != -1)
			{
				stream.WriteInt8(Count);
				stream.WriteInt16(Metadata);
			}
		}
	}
}