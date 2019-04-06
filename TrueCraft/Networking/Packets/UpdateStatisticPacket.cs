namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent to update the client's list of player statistics.
	/// </summary>
	[MessageTarget(MessageTarget.Client)]
	public struct UpdateStatisticPacket : IPacket
	{
		public byte Id => Constants.PacketIds.UpdateStatistic;

		public int StatisticID;
		public sbyte Delta;

		public void ReadPacket(IMcStream stream)
		{
			StatisticID = stream.ReadInt32();
			Delta = stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(StatisticID);
			stream.WriteInt8(Delta);
		}
	}
}