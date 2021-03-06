﻿namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by clients after the handshake to request logging into the world.
	/// </summary>
	[MessageTarget(MessageTarget.Server)]
	public struct LoginRequestPacket : IPacket
	{
		public byte Id => Constants.PacketIds.LoginRequest;

		public LoginRequestPacket(int protocolVersion, string username)
		{
			ProtocolVersion = protocolVersion;
			Username = username;
		}

		public int ProtocolVersion;
		public string Username;

		public void ReadPacket(IMcStream stream)
		{
			ProtocolVersion = stream.ReadInt32();
			Username = stream.ReadString();
			stream.ReadInt64(); // Unused
			stream.ReadInt8(); // Unused
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(ProtocolVersion);
			stream.WriteString(Username);
			stream.WriteInt64(0); // Unused
			stream.WriteInt8(0); // Unused
		}
	}
}