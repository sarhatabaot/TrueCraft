namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by the server to forcibly close an inventory window, or from the client when naturally closed.
	/// </summary>
	public struct CloseWindowPacket : IPacket
	{
		public byte Id => Constants.PacketIds.CloseWindow;

		public CloseWindowPacket(sbyte windowID) => WindowID = windowID;

		public sbyte WindowID;

		public void ReadPacket(IMcStream stream)
		{
			WindowID = stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt8(WindowID);
		}
	}
}