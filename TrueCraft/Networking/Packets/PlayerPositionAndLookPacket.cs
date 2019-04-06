namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by clients to inform the server of updates to their position and look direction.
	///  A combination of the PlayerPosition and PlayerLook packets.
	/// </summary>
	[MessageTarget(MessageTarget.Server)]
	public struct PlayerPositionAndLookPacket : IPacket
	{
		public byte Id => Constants.PacketIds.PlayerPositionAndLook;

		public PlayerPositionAndLookPacket(double x, double y, double stance, double z, float yaw, float pitch,
			bool onGround)
		{
			X = x;
			Y = y;
			Z = z;
			Stance = stance;
			Yaw = yaw;
			Pitch = pitch;
			OnGround = onGround;
		}

		public double X, Y, Z;
		public double Stance;
		public float Yaw, Pitch;
		public bool OnGround;

		public void ReadPacket(IMcStream stream)
		{
			X = stream.ReadDouble();
			Y = stream.ReadDouble();
			Stance = stream.ReadDouble();
			Z = stream.ReadDouble();
			Yaw = stream.ReadSingle();
			Pitch = stream.ReadSingle();
			OnGround = stream.ReadBoolean();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteDouble(X);
			stream.WriteDouble(Y);
			stream.WriteDouble(Stance);
			stream.WriteDouble(Z);
			stream.WriteSingle(Yaw);
			stream.WriteSingle(Pitch);
			stream.WriteBoolean(OnGround);
		}
	}
}