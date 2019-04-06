namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by clients to inform the server of updates to their position.
	/// </summary>
	[MessageTarget(MessageTarget.Server)]
	public struct PlayerPositionPacket : IPacket
	{
		public byte Id => Constants.PacketIds.PlayerPosition;

		public double X, Y, Z;

		/// <summary>
		///  The Y position of the player's eyes. This changes when crouching.
		/// </summary>
		public double Stance;

		public bool OnGround;

		public void ReadPacket(IMcStream stream)
		{
			X = stream.ReadDouble();
			Y = stream.ReadDouble();
			Stance = stream.ReadDouble();
			Z = stream.ReadDouble();
			OnGround = stream.ReadBoolean();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteDouble(X);
			stream.WriteDouble(Y);
			stream.WriteDouble(Stance);
			stream.WriteDouble(Z);
			stream.WriteBoolean(OnGround);
		}
	}
}