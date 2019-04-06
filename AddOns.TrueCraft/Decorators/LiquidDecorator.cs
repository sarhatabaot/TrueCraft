using System;
using TrueCraft.World;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.Decorators
{
	public class LiquidDecorator : IChunkDecorator
	{
		public static readonly int WaterLevel = 40;

		public void Decorate(IWorld world, IChunk chunk, IBiomeRepository biomes)
		{
			for (var x = 0; x < Chunk.Width; x++)
			for (var z = 0; z < Chunk.Depth; z++)
			{
				var biome = biomes.GetBiome(chunk.Biomes[x * Chunk.Width + z]);
				var height = chunk.HeightMap[x * Chunk.Width + z];
				for (var y = height; y <= WaterLevel; y++)
				{
					var blockLocation = new Coordinates3D(x, y, z);
					int blockId = chunk.GetBlockID(blockLocation);
					if (blockId.Equals(AirBlock.BlockId))
					{
						chunk.SetBlockID(blockLocation, biome.WaterBlock);
						var below = blockLocation + Coordinates3D.Down;
						if (!chunk.GetBlockID(below).Equals(AirBlock.BlockId) &&
						    !chunk.GetBlockID(below).Equals(biome.WaterBlock))
							if (!biome.WaterBlock.Equals(LavaBlock.BlockId) &&
							    !biome.WaterBlock.Equals(StationaryLavaBlock.BlockId))
							{
								var random = new Random(world.Seed);
								if (random.Next(100) < 40)
									chunk.SetBlockID(below, ClayBlock.BlockId);
								else
									chunk.SetBlockID(below, SandBlock.BlockId);
							}
					}
				}

				for (var y = 4; y < height / 8; y++)
				{
					var blockLocation = new Coordinates3D(x, y, z);
					int blockId = chunk.GetBlockID(blockLocation);
					if (blockId.Equals(AirBlock.BlockId)) chunk.SetBlockID(blockLocation, LavaBlock.BlockId);
				}
			}
		}
	}
}