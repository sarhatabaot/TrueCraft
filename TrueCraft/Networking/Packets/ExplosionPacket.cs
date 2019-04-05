﻿using System;

namespace TrueCraft.Networking.Packets
{
	public struct ExplosionPacket : IPacket
	{
		public byte ID => 0x3C;

		public double X, Y, Z;
		public float Radius;
		public Tuple<sbyte, sbyte, sbyte>[] AffectedBlocks;

		public void ReadPacket(IMcStream stream)
		{
			X = stream.ReadDouble();
			Y = stream.ReadDouble();
			Z = stream.ReadDouble();
			Radius = stream.ReadSingle();
			AffectedBlocks = new Tuple<sbyte, sbyte, sbyte>[stream.ReadInt32()];
			for (var i = 0; i < AffectedBlocks.Length; i++)
				AffectedBlocks[i] = new Tuple<sbyte, sbyte, sbyte>(
					stream.ReadInt8(),
					stream.ReadInt8(),
					stream.ReadInt8());
		}

		public void WritePacket(IMcStream stream)
		{
			stream.WriteDouble(X);
			stream.WriteDouble(Y);
			stream.WriteDouble(Z);
			stream.WriteSingle(Radius);
			stream.WriteInt32(AffectedBlocks.Length);
			for (var i = 0; i < AffectedBlocks.Length; i++)
			{
				stream.WriteInt8(AffectedBlocks[i].Item1);
				stream.WriteInt8(AffectedBlocks[i].Item2);
				stream.WriteInt8(AffectedBlocks[i].Item3);
			}
		}
	}
}