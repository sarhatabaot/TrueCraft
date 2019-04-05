namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by clients upon clicking an entity.
	/// </summary>
	public struct UseEntityPacket : IPacket
	{
		public byte ID => 0x07;

		public int UserID;
		public int TargetID;
		public bool LeftClick;

		public void ReadPacket(IMcStream stream)
		{
			UserID = stream.ReadInt32();
			TargetID = stream.ReadInt32();
			LeftClick = stream.ReadBoolean();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(UserID);
			stream.WriteInt32(TargetID);
			stream.WriteBoolean(LeftClick);
		}
	}
}