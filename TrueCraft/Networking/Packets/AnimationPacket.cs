﻿namespace TrueCraft.Networking.Packets
{
	public struct AnimationPacket : IPacket
	{
		public enum PlayerAnimation
		{
			None = 0,
			SwingArm = 1,
			TakeDamage = 2,
			LeaveBed = 3,
			Unknown = 102,
			Crouch = 104,
			Uncrouch = 105
		}

		public byte Id => Constants.PacketIds.Animation;

		public int EntityID;
		public PlayerAnimation Animation;

		public AnimationPacket(int entityID, PlayerAnimation animation)
		{
			EntityID = entityID;
			Animation = animation;
		}

		public void ReadPacket(IMcStream stream)
		{
			EntityID = stream.ReadInt32();
			Animation = (PlayerAnimation) stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityID);
			stream.WriteInt8((sbyte) Animation);
		}
	}
}