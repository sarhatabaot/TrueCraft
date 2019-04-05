﻿namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent periodically to confirm that the connection is still active. Send the same packet back
	///  to confirm it. Connection is dropped if no keep alive is received within one minute.
	/// </summary>
	public struct KeepAlivePacket : IPacket
	{
		public byte ID => 0x00;

		public void ReadPacket(IMcStream stream)
		{
			// This space intentionally left blank
		}

		public void WritePacket(IMcStream stream)
		{
			// This space intentionally left blank
		}
	}
}