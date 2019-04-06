namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Plays a sound effect (or special case: generates smoke particles)
	/// </summary>
	[MessageTarget(MessageTarget.Client)]
	public struct SoundEffectPacket : IPacket
	{
		public enum EffectType
		{
			Click2 = 1000,
			Click1 = 1001,
			FireBow = 1002,
			ToggleDoor = 1003,
			Extinguish = 1004,
			PlayRecord = 1005,
			Smoke = 1006,
			BreakBlock = 1007
		}

		public byte Id => Constants.PacketIds.SoundEffect;

		public EffectType Effect;
		public int X;
		public sbyte Y;
		public int Z;

		/// <summary>
		///  For record play, the record ID. For smoke, the direction. For break block, the block ID.
		/// </summary>
		public int Data;

		public void ReadPacket(IMcStream stream)
		{
			Effect = (EffectType) stream.ReadInt32();
			X = stream.ReadInt32();
			Y = stream.ReadInt8();
			Z = stream.ReadInt32();
			Data = stream.ReadInt32();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32((int) Effect);
			stream.WriteInt32(X);
			stream.WriteInt8(Y);
			stream.WriteInt32(Z);
			stream.WriteInt32(Data);
		}
	}
}