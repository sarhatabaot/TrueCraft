namespace TrueCraft.Networking.Packets
{
	[MessageTarget(MessageTarget.Client)]
	public struct DestroyEntityPacket : IPacket
	{
		public byte Id => Constants.PacketIds.DestroyEntity;

		public int EntityId;

		public DestroyEntityPacket(int entityID) => EntityId = entityID;

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