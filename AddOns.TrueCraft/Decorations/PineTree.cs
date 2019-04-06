using System;
using TrueCraft.Logic;
using TrueCraft.Logic.Blocks;
using TrueCraft.World;
using TrueCraft._ADDON.Blocks;
using TrueCraft._ADDON.Decorations;

namespace TrueCraft.Decorations
{
	public class PineTree : Decoration
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
			var height = random.Next(7, 8);
			GenerateColumn(chunk, location, height, WoodBlock.BlockId, 0x1);
			for (var y = 1; y < height; y++)
			{
				if (y % 2 == 0)
				{
					GenerateVanillaCircle(chunk, location + new Coordinates3D(0, y + 1, 0), LeafRadius - 1,
						LeavesBlock.BlockId, 0x1);
					continue;
				}

				GenerateVanillaCircle(chunk, location + new Coordinates3D(0, y + 1, 0), LeafRadius, LeavesBlock.BlockId,
					0x1);
			}

			GenerateTopper(chunk, location + new Coordinates3D(0, height, 0), 0x1);
			return true;
		}

		/*
	     * Generates the top of the pine/conifer trees.
	     * Type:
	     * 0x0 - two level topper
	     * 0x1 - three level topper
	     */
		protected void GenerateTopper(IChunk chunk, Coordinates3D location, byte type = 0x0)
		{
			const int sectionRadius = 1;
			GenerateCircle(chunk, location, sectionRadius, LeavesBlock.BlockId, 0x1);
			var top = location + Coordinates3D.Up;
			chunk.SetBlockID(top, LeavesBlock.BlockId);
			chunk.SetMetadata(top, 0x1);
			if (type == 0x1 && (top + Coordinates3D.Up).Y < Chunk.Height)
				GenerateVanillaCircle(chunk, top + Coordinates3D.Up, sectionRadius, LeavesBlock.BlockId, 0x1);
		}
	}
}