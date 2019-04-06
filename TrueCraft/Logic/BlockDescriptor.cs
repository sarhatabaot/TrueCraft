using TrueCraft.World;

namespace TrueCraft.Logic
{
	public struct BlockDescriptor
	{
		public byte Id;
		public byte Metadata;
		public byte BlockLight;

		public byte SkyLight;

		// Optional
		public Coordinates3D Coordinates;

		// Optional
		public IChunk Chunk;
	}
}