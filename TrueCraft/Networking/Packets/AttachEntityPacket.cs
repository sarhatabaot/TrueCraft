namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Used to attach entities to other entities, i.e. players to minecarts
	/// </summary>
	public struct AttachEntityPacket : IPacket
	{
		public byte ID => 0x27;

		public int EntityID;
		public int VehicleID;

		public void ReadPacket(IMcStream stream)
		{
			EntityID = stream.ReadInt32();
			VehicleID = stream.ReadInt32();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(EntityID);
			stream.WriteInt32(VehicleID);
		}
	}
}