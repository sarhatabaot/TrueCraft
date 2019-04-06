namespace TrueCraft.Networking.Packets
{
	[MessageTarget(MessageTarget.Server)]
	public struct PlayerActionPacket : IPacket
	{
		public enum PlayerAction
		{
			Crouch = 1,
			Uncrouch = 2,
			LeaveBed = 3
		}

		public byte Id => Constants.PacketIds.PlayerAction;

		public int EntityId;
		public PlayerAction Action;

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			Action = (PlayerAction) stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			stream.WriteInt8((sbyte) Action);
		}
	}
}