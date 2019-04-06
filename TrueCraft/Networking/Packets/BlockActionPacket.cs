﻿namespace TrueCraft.Networking.Packets
{
	/// <summary>
	///  Used to indicate that a certain "action" has happened to a block.
	///  In practice this controls note blocks and pistons.
	/// </summary>
	public struct BlockActionPacket : IPacket
	{
		public byte Id => Constants.PacketIds.BlockAction;

		public int X;
		public short Y;
		public int Z;

		/// <summary>
		///  Used for instrument type or piston state.
		/// </summary>
		public sbyte State;

		/// <summary>
		///  Used for piston direction or note block instrument.
		/// </summary>
		public sbyte Data;

		public void ReadPacket(IMcStream stream)
		{
			X = stream.ReadInt32();
			Y = stream.ReadInt16();
			Z = stream.ReadInt32();
			State = stream.ReadInt8();
			Data = stream.ReadInt8();
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteInt32(X);
			stream.WriteInt32(Y);
			stream.WriteInt32(Z);
			stream.WriteInt8(State);
			stream.WriteInt8(Data);
		}
	}
}