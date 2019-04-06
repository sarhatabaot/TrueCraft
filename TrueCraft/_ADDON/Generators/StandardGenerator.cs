﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TrueCraft.Logic.Blocks;
using TrueCraft.TerrainGen.Decorators;
using TrueCraft.TerrainGen.Noise;
using TrueCraft.World;

namespace TrueCraft.TerrainGen
{
	/// <summary>
	///  This terrain generator is still under heavy development. Use at your own risk.
	/// </summary>
	public class StandardGenerator : IChunkProvider
	{
		private const int GroundLevel = 50;
		private readonly BiomeRepository Biomes = new BiomeRepository();
		private ClampNoise BottomClamp;
		private Perlin BottomNoise;
		private Perlin CaveNoise;
		private readonly bool EnableCaves;
		private ModifyNoise FinalNoise;
		private ClampNoise HighClamp;
		private Perlin HighNoise;
		private ClampNoise LowClamp;
		private Perlin LowNoise;

		public StandardGenerator()
		{
			EnableCaves = true;
			ChunkDecorators = new List<IChunkDecorator>();
			ChunkDecorators.Add(new LiquidDecorator());
			ChunkDecorators.Add(new OreDecorator());
			ChunkDecorators.Add(new PlantDecorator());
			ChunkDecorators.Add(new TreeDecorator());
			ChunkDecorators.Add(new FreezeDecorator());
			ChunkDecorators.Add(new CactusDecorator());
			ChunkDecorators.Add(new SugarCaneDecorator());
			ChunkDecorators.Add(new DungeonDecorator(GroundLevel));
		}

		public Vector3 SpawnPoint { get; private set; }
		public bool SingleBiome { get; private set; }
		public byte GenerationBiome { get; private set; }

		public void Initialize(IWorld world)
		{
			HighNoise = new Perlin(world.Seed);
			LowNoise = new Perlin(world.Seed);
			BottomNoise = new Perlin(world.Seed);
			CaveNoise = new Perlin(world.Seed);

			CaveNoise.Octaves = 3;
			CaveNoise.Amplitude = 0.05;
			CaveNoise.Persistance = 2;
			CaveNoise.Frequency = 0.05;
			CaveNoise.Lacunarity = 2;

			HighNoise.Persistance = 1;
			HighNoise.Frequency = 0.013;
			HighNoise.Amplitude = 10;
			HighNoise.Octaves = 2;
			HighNoise.Lacunarity = 2;

			LowNoise.Persistance = 1;
			LowNoise.Frequency = 0.004;
			LowNoise.Amplitude = 35;
			LowNoise.Octaves = 2;
			LowNoise.Lacunarity = 2.5;

			BottomNoise.Persistance = 0.5;
			BottomNoise.Frequency = 0.013;
			BottomNoise.Amplitude = 5;
			BottomNoise.Octaves = 2;
			BottomNoise.Lacunarity = 1.5;

			HighClamp = new ClampNoise(HighNoise);
			HighClamp.MinValue = -30;
			HighClamp.MaxValue = 50;

			LowClamp = new ClampNoise(LowNoise);
			LowClamp.MinValue = -30;
			LowClamp.MaxValue = 30;

			BottomClamp = new ClampNoise(BottomNoise);
			BottomClamp.MinValue = -20;
			BottomClamp.MaxValue = 5;

			FinalNoise = new ModifyNoise(HighClamp, LowClamp, NoiseModifier.Add);
		}

		public IList<IChunkDecorator> ChunkDecorators { get; }

		public IChunk GenerateChunk(IWorld world, Coordinates2D coordinates)
		{
			const int featurePointDistance = 400;

			// TODO: Create a terrain generator initializer function that gets passed the seed etc
			var seed = world.Seed;
			var worley = new CellNoise(seed);
			HighNoise.Seed = seed;
			LowNoise.Seed = seed;
			CaveNoise.Seed = seed;

			var chunk = new Chunk(coordinates);

			for (var x = 0; x < 16; x++)
			for (var z = 0; z < 16; z++)
			{
				var blockX = MathHelper.ChunkToBlockX(x, coordinates.X);
				var blockZ = MathHelper.ChunkToBlockZ(z, coordinates.Z);

				const double lowClampRange = 5;
				var lowClampMid = LowClamp.MaxValue - (LowClamp.MaxValue + LowClamp.MinValue) / 2;
				var lowClampValue = LowClamp.Value2D(blockX, blockZ);

				if (lowClampValue > lowClampMid - lowClampRange && lowClampValue < lowClampMid + lowClampRange)
				{
					var NewPrimary = new InvertNoise(HighClamp);
					FinalNoise.PrimaryNoise = NewPrimary;
				}
				else
					FinalNoise = new ModifyNoise(HighClamp, LowClamp, NoiseModifier.Add);

				FinalNoise = new ModifyNoise(FinalNoise, BottomClamp, NoiseModifier.Subtract);

				var cellValue = worley.Value2D(blockX, blockZ);
				var location = new Coordinates2D(blockX, blockZ);
				if (world.BiomeDiagram.BiomeCells.Count < 1
				    || cellValue.Equals(1)
				    && world.BiomeDiagram.ClosestCellPoint(location) >= featurePointDistance)
				{
					var Id = SingleBiome
						? GenerationBiome
						: world.BiomeDiagram.GenerateBiome(seed, Biomes, location,
							IsSpawnCoordinate(location.X, location.Z));
					var cell = new BiomeCell(Id, location);
					world.BiomeDiagram.AddCell(cell);
				}

				var biomeId = GetBiome(world, location);
				var biome = Biomes.GetBiome(biomeId);
				chunk.Biomes[x * Chunk.Width + z] = biomeId;

				var height = GetHeight(blockX, blockZ);
				var surfaceHeight = height - biome.SurfaceDepth;
				chunk.HeightMap[x * Chunk.Width + z] = height;

				// TODO: Do not overwrite blocks if they are already set from adjacent chunks
				for (var y = 0; y <= height; y++)
				{
					double cave = 0;
					if (!EnableCaves)
						cave = double.MaxValue;
					else
						cave = CaveNoise.Value3D((blockX + x) / 2, y / 2, (blockZ + z) / 2);
					var threshold = 0.05;
					if (y < 4)
						threshold = double.MaxValue;
					else
					{
						if (y > height - 8)
							threshold = 8;
					}

					if (cave < threshold)
					{
						if (y == 0)
							chunk.SetBlockID(new Coordinates3D(x, y, z), BedrockBlock.BlockId);
						else
						{
							if (y.Equals(height) || y < height && y > surfaceHeight)
								chunk.SetBlockID(new Coordinates3D(x, y, z), biome.SurfaceBlock);
							else
							{
								if (y > surfaceHeight - biome.FillerDepth)
									chunk.SetBlockID(new Coordinates3D(x, y, z), biome.FillerBlock);
								else
									chunk.SetBlockID(new Coordinates3D(x, y, z), StoneBlock.BlockId);
							}
						}
					}
				}
			}

			foreach (var decorator in ChunkDecorators)
				decorator.Decorate(world, chunk, Biomes);
			chunk.TerrainPopulated = true;
			chunk.UpdateHeightMap();
			return chunk;
		}

		public Coordinates3D GetSpawn(IWorld world)
		{
			var chunk = GenerateChunk(world, Coordinates2D.Zero);
			var spawnPointHeight = chunk.HeightMap[0];
			return new Coordinates3D(0, spawnPointHeight + 1, 0);
		}

		private byte GetBiome(IWorld world, Coordinates2D location)
		{
			if (SingleBiome)
				return GenerationBiome;
			return world.BiomeDiagram.GetBiome(location);
		}

		private bool IsSpawnCoordinate(int x, int z)
		{
			return x > -1000 && x < 1000 || z > -1000 && z < 1000;
		}

		private int GetHeight(int x, int z)
		{
			var value = FinalNoise.Value2D(x, z) + GroundLevel;
			var coords = new Coordinates2D(x, z);
			var distance = IsSpawnCoordinate(x, z) ? coords.Distance : 1000;
			if (distance < 1000) // Avoids deep water within 1km sq of spawn
				value += (1 - distance / 1000f) * 18;
			if (value < 0)
				value = GroundLevel;
			if (value > Chunk.Height)
				value = Chunk.Height - 1;
			return (int) value;
		}
	}
}