namespace TrueCraft.Networking.Packets
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

		public int EntityId;
		public PlayerAnimation Animation;

		public AnimationPacket(int entityID, PlayerAnimation animation)
		{
			EntityId = entityID;
			Animation = animation;
		}

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			Animation = (PlayerAnimation) stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			stream.WriteInt8((sbyte) Animation);
		}
	}
}