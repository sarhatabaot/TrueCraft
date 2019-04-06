namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent to update the rotation of the player's head and body.
	/// </summary>
	public struct PlayerLookPacket : IPacket
	{
		public byte Id => Constants.PacketIds.PlayerLook;

		public float Yaw, Pitch;
		public bool OnGround;

		public void ReadPacket(IMcStream stream)
		{
			Yaw = stream.ReadSingle();
			Pitch = stream.ReadSingle();
			OnGround = stream.ReadBoolean();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteSingle(Yaw);
			stream.WriteSingle(Pitch);
			stream.WriteBoolean(OnGround);
		}
	}
}