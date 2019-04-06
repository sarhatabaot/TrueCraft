﻿namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Used to attach entities to other entities, i.e. players to carts
	/// </summary>
	[MessageTarget(MessageTarget.Client)]
	public struct AttachEntityPacket : IPacket
	{
		public byte Id => Constants.PacketIds.AttachEntity;

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