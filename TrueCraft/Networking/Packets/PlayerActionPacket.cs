﻿namespace TrueCraft.Networking.Packets
{
	public struct PlayerActionPacket : IPacket
	{
		public enum PlayerAction
		{
			Crouch = 1,
			Uncrouch = 2,
			LeaveBed = 3
		}

		public byte ID => 0x13;

		public int EntityID;
		public PlayerAction Action;

		public void ReadPacket(IMcStream stream)
		{
			EntityID = stream.ReadInt32();
			Action = (PlayerAction) stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityID);
			stream.WriteInt8((sbyte) Action);
		}
	}
}