using System;
using TrueCraft.Logic.Blocks;
using TrueCraft.World;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft._ADDON.Decorations
{
	public class OakTree : Decoration
	{
		private const int LeafRadius = 2;

		public override bool ValidLocation(Coordinates3D location)
		{
			if (location.X - LeafRadius < 0
			    || location.X + LeafRadius >= Chunk.Width
			    || location.Z - LeafRadius < 0
			    || location.Z + LeafRadius >= Chunk.Depth)
				return false;
			return true;
		}

		public override bool GenerateAt(IWorld world, IChunk chunk, Coordinates3D location)
		{
			if (!ValidLocation(location))
				return false;

			var random = new Random(world.Seed);
			var height = random.Next(4, 5);
			GenerateColumn(chunk, location, height, WoodBlock.BlockId, 0x0);
			var LeafLocation = location + new Coordinates3D(0, height, 0);
			GenerateVanillaLeaves(chunk, LeafLocation, LeafRadius, LeavesBlock.BlockId, 0x0);
			return true;
		}
	}
}