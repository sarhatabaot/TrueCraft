using System;
using System.Collections.Generic;
using System.Linq;
using TrueCraft.Logic.Blocks;
using TrueCraft.TerrainGen.Noise;
using TrueCraft.World;

namespace TrueCraft.TerrainGen.Decorators
{
	public class OreDecorator : IChunkDecorator
	{
		private readonly List<OreData> Ores = new List<OreData>();

		public OreDecorator()
		{
			var coal = new OreData(CoalOreBlock.BlockId, OreTypes.Coal, 10, 120, 25, 25, 3f);
			var iron = new OreData(IronOreBlock.BlockId, OreTypes.Iron, 1, 64, 15, 5, 2.3f);
			var lapis = new OreData(LapisLazuliOreBlock.BlockId, OreTypes.Lapiz, 10, 25, 7, 4, 1.4f);
			var gold = new OreData(GoldOreBlock.BlockId, OreTypes.Gold, 1, 32, 6, 4, 1.04f);
			var diamond = new OreData(DiamondOreBlock.BlockId, OreTypes.Diamond, 1, 15, 6, 3, 0.7f);
			var redstone = new OreData(RedstoneOreBlock.BlockId, OreTypes.Redstone, 1, 16, 4, 6, 1.13f);
			Ores.Add(coal);
			Ores.Add(iron);
			Ores.Add(lapis);
			Ores.Add(gold);
			Ores.Add(diamond);
			Ores.Add(redstone);
		}

		public void Decorate(IWorld world, IChunk chunk, IBiomeRepository biomes)
		{
			var perlin = new Perlin(world.Seed);
			perlin.Lacunarity = 1;
			perlin.Amplitude = 7;
			perlin.Frequency = 0.015;
			var chanceNoise = new ClampNoise(perlin);
			var noise = new ScaledNoise(perlin);
			var random = new Random(world.Seed);
			var lowWeightOffset = new int[2] {2, 3};
			var highWeightOffset = new int[2] {2, 2};
			foreach (var data in Ores)
			{
				var midpoint = (data.MaxY + data.MinY) / 2;
				var weightOffsets = data.MaxY > 30 ? highWeightOffset : lowWeightOffset;
				const int weightPasses = 4;
				for (var i = 0; i < data.Veins; i++)
				{
					double weight = 0;
					for (var j = 0; j < weightPasses; j++) weight += random.NextDouble();
					weight /= data.Rarity;
					weight = weightOffsets[0] - Math.Abs(weight - weightOffsets[1]);
					double x = random.Next(0, Chunk.Width);
					double z = random.Next(0, Chunk.Depth);
					var y = weight * midpoint;

					double randomOffsetX = (float) random.NextDouble() - 1;
					double randomOffsetY = (float) random.NextDouble() - 1;
					double randomOffsetZ = (float) random.NextDouble() - 1;

					var abundance = random.Next(0, data.Abundance);
					for (var k = 0; k < abundance; k++)
					{
						x += randomOffsetX;
						y += randomOffsetY;
						z += randomOffsetZ;
						if (x >= 0 && z >= 0 && y >= data.MinY && x < Chunk.Width && y < data.MaxY && z < Chunk.Depth)
						{
							var biome = biomes.GetBiome(chunk.Biomes[(int) (x * Chunk.Width + z)]);
							if (biome.Ores.Contains(data.Type) && chunk
								    .GetBlockID(new Coordinates3D((int) x, (int) y, (int) z))
								    .Equals(StoneBlock.BlockId))
								chunk.SetBlockID(new Coordinates3D((int) x, (int) y, (int) z), data.Id);
						}

						var blockX = MathHelper.ChunkToBlockX((int) x, chunk.Coordinates.X);
						var blockZ = MathHelper.ChunkToBlockZ((int) z, chunk.Coordinates.Z);

						double offsetX = 0;
						double offsetY = 0;
						double offsetZ = 0;
						var offset = random.Next(0, 3);
						var offset2 = random.NextDouble();

						if (offset.Equals(0) && offset2 < 0.4)
							offsetX += 1;
						else if (offset.Equals(1) && offset2 >= 0.4 && offset2 < 0.65)
							offsetY += 1;
						else
							offsetZ += 1;

						var newX = (int) (x + offsetX);
						var newY = (int) (y + offsetY);
						var newZ = (int) (z + offsetZ);
						if (newX >= 0 && newZ >= 0 && newY >= data.MinY && newX < Chunk.Width && newY < data.MaxY &&
						    newZ < Chunk.Depth)
						{
							var Biome = biomes.GetBiome(chunk.Biomes[newX * Chunk.Width + newZ]);
							var coordinates = new Coordinates3D(newX, newY, newZ);
							if (Biome.Ores.Contains(data.Type) &&
							    chunk.GetBlockID(coordinates).Equals(StoneBlock.BlockId))
								chunk.SetBlockID(coordinates, data.Id);
						}
					}
				}
			}
		}
	}
}