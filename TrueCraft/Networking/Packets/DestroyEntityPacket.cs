﻿namespace TrueCraft.Networking.Packets
{
	public struct DestroyEntityPacket : IPacket
	{
		public byte ID => 0x1D;

		public int EntityID;

		public DestroyEntityPacket(int entityID) => EntityID = entityID;

		public void ReadPacket(IMinecraftStream stream)
		{
			EntityID = stream.ReadInt32();
		}

		public void WritePacket(IMinecraftStream stream)
		{
			stream.WriteInt32(EntityID);
		}
	}
}