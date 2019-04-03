using System;
using System.Collections.ObjectModel;
using TrueCraft.World;

namespace TrueCraft.Client
{
	public class ReadOnlyChunk
	{
		internal ReadOnlyChunk(IChunk chunk) => Chunk = chunk;

		internal IChunk Chunk { get; set; }

		public Coordinates2D Coordinates => Chunk.Coordinates;

		public int X => Chunk.X;
		public int Z => Chunk.Z;

		public ReadOnlyCollection<byte> Blocks => Array.AsReadOnly(Chunk.Data);
		public ReadOnlyNibbleArray Metadata => new ReadOnlyNibbleArray(Chunk.Metadata);
		public ReadOnlyNibbleArray BlockLight => new ReadOnlyNibbleArray(Chunk.BlockLight);
		public ReadOnlyNibbleArray SkyLight => new ReadOnlyNibbleArray(Chunk.SkyLight);

		public byte GetBlockId(Coordinates3D coordinates)
		{
			return Chunk.GetBlockID(coordinates);
		}

		public byte GetMetadata(Coordinates3D coordinates)
		{
			return Chunk.GetMetadata(coordinates);
		}

		public byte GetSkyLight(Coordinates3D coordinates)
		{
			return Chunk.GetSkyLight(coordinates);
		}

		public byte GetBlockLight(Coordinates3D coordinates)
		{
			return Chunk.GetBlockLight(coordinates);
		}
	}
}