﻿namespace TrueCraft.Networking.Packets
{
	[MessageTarget(MessageTarget.Client)]
	public struct MapDataPacket : IPacket
	{
		public byte Id => Constants.PacketIds.MapData;

		public short ItemID;
		public short Metadata;
		public byte[] Data;

		public void ReadPacket(IMcStream stream)
		{
			ItemID = stream.ReadInt16();
			Metadata = stream.ReadInt16();
			var length = stream.ReadUInt8();
			Data = stream.ReadUInt8Array(length);
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt16(ItemID);
			stream.WriteInt16(Metadata);
			stream.WriteUInt8((byte) Data.Length);
			stream.WriteUInt8Array(Data);
		}
	}
}