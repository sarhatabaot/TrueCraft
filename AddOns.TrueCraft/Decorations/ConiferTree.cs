using System;
using TrueCraft.Logic.Blocks;
using TrueCraft.World;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.Decorations
{
	public class ConiferTree : PineTree
	{
		private const int LeafRadius = 2;

		public override bool GenerateAt(IWorld world, IChunk chunk, Coordinates3D location)
		{
			if (!ValidLocation(location))
				return false;

			var random = new Random(world.Seed);
			var height = random.Next(7, 8);
			GenerateColumn(chunk, location, height, WoodBlock.BlockId, 0x1);
			GenerateCircle(chunk, location + new Coordinates3D(0, height - 2, 0), LeafRadius - 1, LeavesBlock.BlockId,
				0x1);
			GenerateCircle(chunk, location + new Coordinates3D(0, height - 1, 0), LeafRadius, LeavesBlock.BlockId, 0x1);
			GenerateCircle(chunk, location + new Coordinates3D(0, height, 0), LeafRadius, LeavesBlock.BlockId, 0x1);
			GenerateTopper(chunk, location + new Coordinates3D(0, height + 1, 0), 0x0);
			return true;
		}
	}
}