namespace TrueCraft.Networking.Packets
{
	public struct DestroyEntityPacket : IPacket
	{
		public byte Id => Constants.PacketIds.DestroyEntity;

		public int EntityID;

		public DestroyEntityPacket(int entityID) => EntityID = entityID;

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