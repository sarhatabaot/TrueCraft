namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Gives updates to what entities are doing.
	/// </summary>
	[MessageTarget(MessageTarget.Client)]
	public struct EntityStatusPacket : IPacket
	{
		public enum EntityStatus
		{
			EntityHurt = 2,
			EntityKilled = 3,
			WolfTaming = 6,
			WolfTamed = 7,
			WolfShaking = 8,
			EatingAccepted = 9 // what
		}

		public byte Id => Constants.PacketIds.EntityStatus;

		public int EntityId;
		public EntityStatus Status;

		public void ReadPacket(IMcStream stream)
		{
			EntityId = stream.ReadInt32();
			Status = (EntityStatus) stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityId);
			stream.WriteInt8((sbyte) Status);
		}
	}
}