namespace TrueCraft.Networking.Packets
{
	[MessageTarget(MessageTarget.Client)]
	public struct UselessEntityPacket : IPacket
	{
		public byte Id => Constants.PacketIds.UselessEntity;

		public int EntityId;

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
		}
	}
}