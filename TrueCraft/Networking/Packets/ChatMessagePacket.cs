namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Used by clients to send messages and by servers to propegate messages to clients.
	///  Note that the server is expected to include the username, i.e.
	///  <User>
	///   message, but the
	///   client is not given the same expectation.
	/// </summary>
	public struct ChatMessagePacket : IPacket
	{
		public byte Id => Constants.PacketIds.ChatMessage;

		public ChatMessagePacket(string message) => Message = message;

		public string Message;

		public void ReadPacket(IMcStream stream)
		{
			Message = stream.ReadString();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteString(Message);
		}
	}
}