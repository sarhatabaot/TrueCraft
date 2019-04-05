﻿namespace TrueCraft.Networking.Packets
{
	public struct ChangeHeldItemPacket : IPacket
	{
		public byte ID => 0x10;

		public short Slot;

		public void ReadPacket(IMcStream stream)
		{
			Slot = stream.ReadInt16();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt16(Slot);
		}
	}
}