using System;

namespace TrueCraft.World
{
	public class ChunkLoadedEventArgs : EventArgs
	{
		public ChunkLoadedEventArgs(IChunk chunk)
		{
			Chunk = chunk;
			Coordinates = chunk.Coordinates;
		}

		public Coordinates2D Coordinates { get; set; }
		public IChunk Chunk { get; set; }
	}
}