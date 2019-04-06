namespace TrueCraft.Networking.Packets
{
	public struct ChangeHeldItemPacket : IPacket
	{
		public byte Id => Constants.PacketIds.ChangeHeldItem;

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