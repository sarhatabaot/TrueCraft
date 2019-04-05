namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Sent by clients to update whether or not the player is on the ground. Probably best to just ignore this.
	/// </summary>
	public struct PlayerGroundedPacket : IPacket
	{
		public byte ID => 0x0A;

		public bool OnGround;

		public void ReadPacket(IMcStream stream)
		{
			OnGround = stream.ReadBoolean();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteBoolean(OnGround);
		}
	}
}