namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by servers to show the animation of an item entity being collected by a player.
	/// </summary>
	public struct CollectItemPacket : IPacket
	{
		public byte Id => Constants.PacketIds.CollectItem;

		public int CollectedItemID;
		public int CollectorID;

		public CollectItemPacket(int collectedItemID, int collectorID)
		{
			CollectedItemID = collectedItemID;
			CollectorID = collectorID;
		}

		public void ReadPacket(IMcStream stream)
		{
			CollectedItemID = stream.ReadInt32();
			CollectorID = stream.ReadInt32();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(CollectedItemID);
			stream.WriteInt32(CollectorID);
		}
	}
}