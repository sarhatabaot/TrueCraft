﻿namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by clients when clicking on an inventory window.
	/// </summary>
	[MessageTarget(MessageTarget.Server)]
	public struct ClickWindowPacket : IPacket
	{
		public byte Id => Constants.PacketIds.ClickWindow;

		public ClickWindowPacket(sbyte windowID, short slotIndex, bool rightClick, short transactionID, bool shift,
			short itemID, sbyte count, short metadata)
		{
			WindowID = windowID;
			SlotIndex = slotIndex;
			RightClick = rightClick;
			TransactionID = transactionID;
			Shift = shift;
			ItemId = itemID;
			Count = count;
			Metadata = metadata;
		}

		public sbyte WindowID;
		public short SlotIndex;
		public bool RightClick;
		public short TransactionID;
		public bool Shift;

		/// <summary>
		///  You should probably ignore this.
		/// </summary>
		public short ItemId;

		/// <summary>
		///  You should probably ignore this.
		/// </summary>
		public sbyte Count;

		/// <summary>
		///  You should probably ignore this.
		/// </summary>
		public short Metadata;

		public void ReadPacket(IMcStream stream)
		{
			WindowID = stream.ReadInt8();
			SlotIndex = stream.ReadInt16();
			RightClick = stream.ReadBoolean();
			TransactionID = stream.ReadInt16();
			Shift = stream.ReadBoolean();
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
			stream.WriteBoolean(RightClick);
			stream.WriteInt16(TransactionID);
			stream.WriteBoolean(Shift);
			stream.WriteInt16(ItemId);
			if (ItemId != -1)
			{
				stream.WriteInt8(Count);
				stream.WriteInt16(Metadata);
			}
		}
	}
}