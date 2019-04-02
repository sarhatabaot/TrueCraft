﻿using System;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace fNbt
{
	/// <summary>
	///  BinaryReader wrapper that takes care of reading primitives from an NBT stream,
	///  while taking care of endianness, string encoding, and skipping.
	/// </summary>
	internal sealed class NbtBinaryReader : BinaryReader
	{
		private const int SeekBufferSize = 64 * 1024;
		private readonly bool bigEndian;

		private readonly byte[] floatBuffer = new byte[sizeof(float)],
			doubleBuffer = new byte[sizeof(double)];

		private byte[] seekBuffer;


		public NbtBinaryReader([NotNull] Stream input, bool bigEndian)
			: base(input) =>
			this.bigEndian = bigEndian;


		public TagSelector Selector { get; set; }


		public NbtTagType ReadTagType()
		{
			var type = (NbtTagType) ReadByte();
			if (type < NbtTagType.End || type > NbtTagType.IntArray)
				throw new NbtFormatException("NBT tag type out of range: " + (int) type);
			return type;
		}


		public override short ReadInt16()
		{
			if (BitConverter.IsLittleEndian == bigEndian)
				return NbtBinaryWriter.Swap(base.ReadInt16());
			return base.ReadInt16();
		}


		public override int ReadInt32()
		{
			if (BitConverter.IsLittleEndian == bigEndian)
				return NbtBinaryWriter.Swap(base.ReadInt32());
			return base.ReadInt32();
		}


		public override long ReadInt64()
		{
			if (BitConverter.IsLittleEndian == bigEndian)
				return NbtBinaryWriter.Swap(base.ReadInt64());
			return base.ReadInt64();
		}


		public override float ReadSingle()
		{
			if (BitConverter.IsLittleEndian == bigEndian)
			{
				BaseStream.Read(floatBuffer, 0, sizeof(float));
				Array.Reverse(floatBuffer);
				return BitConverter.ToSingle(floatBuffer, 0);
			}

			return base.ReadSingle();
		}


		public override double ReadDouble()
		{
			if (BitConverter.IsLittleEndian == bigEndian)
			{
				BaseStream.Read(doubleBuffer, 0, sizeof(double));
				Array.Reverse(doubleBuffer);
				return BitConverter.ToDouble(doubleBuffer, 0);
			}

			return base.ReadDouble();
		}


		public override string ReadString()
		{
			var length = ReadInt16();
			if (length < 0) throw new NbtFormatException("Negative string length given!");
			var stringData = ReadBytes(length);
			return Encoding.UTF8.GetString(stringData);
		}


		public void Skip(int bytesToSkip)
		{
			if (bytesToSkip < 0)
				throw new ArgumentOutOfRangeException("bytesToSkip");
			if (BaseStream.CanSeek)
				BaseStream.Position += bytesToSkip;
			else if (bytesToSkip != 0)
			{
				if (seekBuffer == null)
					seekBuffer = new byte[SeekBufferSize];
				var bytesDone = 0;
				while (bytesDone < bytesToSkip)
				{
					var readThisTime = BaseStream.Read(seekBuffer, bytesDone, bytesToSkip - bytesDone);
					if (readThisTime == 0) throw new EndOfStreamException();
					bytesDone += readThisTime;
				}
			}
		}


		public void SkipString()
		{
			var length = ReadInt16();
			if (length < 0) throw new NbtFormatException("Negative string length given!");
			Skip(length);
		}
	}
}