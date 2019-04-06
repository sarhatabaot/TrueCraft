namespace TrueCraft.Networking.Packets
{
	[MessageTarget(MessageTarget.Client)]
	public struct EntityMetadataPacket : IPacket
	{
		public byte Id => Constants.PacketIds.EntityMetadata;

		public int EntityId;
		public MetadataDictionary Metadata;

		public EntityMetadataPacket(int entityID, MetadataDictionary metadata)
		{
			EntityId = entityID;
			Metadata = metadata;
		}

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			Metadata = MetadataDictionary.FromStream(stream);
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			Metadata.WriteTo(stream);
		}
	}
}