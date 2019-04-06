namespace TrueCraft.Networking.Packets
{
	public struct EntityMetadataPacket : IPacket
	{
		public byte Id => Constants.PacketIds.EntityMetadata;

		public int EntityID;
		public MetadataDictionary Metadata;

		public EntityMetadataPacket(int entityID, MetadataDictionary metadata)
		{
			EntityID = entityID;
			Metadata = metadata;
		}

		public void ReadPacket(IMcStream stream)
		{
			EntityID = stream.ReadInt32();
			Metadata = MetadataDictionary.FromStream(stream);
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityID);
			Metadata.WriteTo(stream);
		}
	}
}