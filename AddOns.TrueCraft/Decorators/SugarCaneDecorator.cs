using System;
using System.Linq;
using TrueCraft.Logic;
using TrueCraft.TerrainGen.Noise;
using TrueCraft.World;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.Decorators
{
	public class SugarCaneDecorator : IChunkDecorator
	{
		public void Decorate(IWorld world, IChunk chunk, IBiomeRepository biomes)
		{
			var noise = new Perlin(world.Seed);
			var chanceNoise = new ClampNoise(noise);
			chanceNoise.MaxValue = 1;
			for (var x = 0; x < 16; x++)
			for (var z = 0; z < 16; z++)
			{
				var biome = biomes.GetBiome(chunk.Biomes[x * Chunk.Width + z]);
				var height = chunk.HeightMap[x * Chunk.Width + z];
				var blockX = MathHelper.ChunkToBlockX(x, chunk.Coordinates.X);
				var blockZ = MathHelper.ChunkToBlockZ(z, chunk.Coordinates.Z);
				if (biome.Plants.Contains(PlantSpecies.SugarCane))
					if (noise.Value2D(blockX, blockZ) > 0.65)
					{
						var blockLocation = new Coordinates3D(x, height, z);
						var sugarCaneLocation = blockLocation + Coordinates3D.Up;
						var neighborsWater = Decoration.NeighboursBlock(chunk, blockLocation, WaterBlock.BlockId) ||
						                     Decoration.NeighboursBlock(chunk, blockLocation,
							                     StationaryWaterBlock.BlockId);
						if (chunk.GetBlockID(blockLocation).Equals(GrassBlock.BlockId) && neighborsWater ||
						    chunk.GetBlockID(blockLocation).Equals(SandBlock.BlockId) && neighborsWater)
						{
							var random = new Random(world.Seed);
							var heightChance = random.NextDouble();
							var caneHeight = 3;
							if (heightChance < 0.05)
								caneHeight = 4;
							else if (heightChance > 0.1 && height < 0.25)
								caneHeight = 2;
							Decoration.GenerateColumn(chunk, sugarCaneLocation, caneHeight, SugarcaneBlock.BlockId);
						}
					}
			}
		}
	}
}