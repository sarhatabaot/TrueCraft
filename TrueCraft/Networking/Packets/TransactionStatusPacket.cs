namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by the server to inform the client if an inventory transaction was successful.
	/// </summary>
	[MessageTarget(MessageTarget.Client)]
	public struct TransactionStatusPacket : IPacket
	{
		public byte Id => Constants.PacketIds.TransactionStatus;

		public sbyte WindowID;
		public short TransactionID;
		public bool Accepted;

		public void ReadPacket(IMcStream stream)
		{
			WindowID = stream.ReadInt8();
			TransactionID = stream.ReadInt16();
			Accepted = stream.ReadBoolean();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt8(WindowID);
			stream.WriteInt16(TransactionID);
			stream.WriteBoolean(Accepted);
		}
	}
}