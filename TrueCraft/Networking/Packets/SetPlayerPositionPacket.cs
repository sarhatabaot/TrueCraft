namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by servers to set the position and look of the player. Can be used to teleport players.
	/// </summary>
	public struct SetPlayerPositionPacket : IPacket
	{
		public byte Id => Constants.PacketIds.SetPlayerPosition;

		public SetPlayerPositionPacket(double x, double y, double stance, double z, float yaw, float pitch,
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
			Stance = stream.ReadDouble();
			Y = stream.ReadDouble();
			Z = stream.ReadDouble();
			Yaw = stream.ReadSingle();
			Pitch = stream.ReadSingle();
			OnGround = stream.ReadBoolean();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteDouble(X);
			stream.WriteDouble(Stance);
			stream.WriteDouble(Y);
			stream.WriteDouble(Z);
			stream.WriteSingle(Yaw);
			stream.WriteSingle(Pitch);
			stream.WriteBoolean(OnGround);
		}
	}
}