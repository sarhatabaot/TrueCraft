﻿namespace TrueCraft.Networking
{
	public interface IPacket
	{
		byte ID { get; }
		void ReadPacket(IMinecraftStream stream);
		void WritePacket(IMinecraftStream stream);
	}
}