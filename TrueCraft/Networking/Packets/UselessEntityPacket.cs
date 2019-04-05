namespace TrueCraft.Networking.Packets
{
	public struct UselessEntityPacket : IPacket
	{
		public byte ID => 0x1E;

		public int EntityID;

		public void ReadPacket(IMcStream stream)
		{
			EntityID = stream.ReadInt32();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityID);
		}
	}
}