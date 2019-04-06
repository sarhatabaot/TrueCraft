namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Updates the player on changes to or status of the environment.
	/// </summary>
	public struct EnvironmentStatePacket : IPacket
	{
		public enum EnvironmentState
		{
			InvalidBed = 0,
			BeginRaining = 1,
			EndRaining = 2
		}

		public byte Id => Constants.PacketIds.EnvironmentState;

		public EnvironmentState State;

		public void ReadPacket(IMcStream stream)
		{
			State = (EnvironmentState) stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt8((sbyte) State);
		}
	}
}