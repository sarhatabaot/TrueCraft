using System.Linq;
using TrueCraft.Logic.Blocks;
using TrueCraft.World;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft._ADDON.Decorators
{
	internal class FreezeDecorator : IChunkDecorator
	{
		public void Decorate(IWorld world, IChunk chunk, IBiomeRepository biomes)
		{
			for (var x = 0; x < 16; x++)
			for (var z = 0; z < 16; z++)
			{
				var biome = biomes.GetBiome(chunk.Biomes[x * Chunk.Width + z]);
				if (biome.Temperature < 0.15)
				{
					var height = chunk.HeightMap[x * Chunk.Width + z];
					for (var y = height; y < Chunk.Height; y++)
					{
						var location = new Coordinates3D(x, y, z);
						if (chunk.GetBlockID(location).Equals(StationaryWaterBlock.BlockId) ||
						    chunk.GetBlockID(location).Equals(WaterBlock.BlockId))
							chunk.SetBlockID(location, IceBlock.BlockId);
						else
						{
							var below = chunk.GetBlockID(location);
							byte[] whitelist =
							{
								DirtBlock.BlockId,
								GrassBlock.BlockId,
								IceBlock.BlockId,
								LeavesBlock.BlockId
							};
							if (y == height && whitelist.Any(w => w == below))
							{
								if (chunk.GetBlockID(location).Equals(IceBlock.BlockId) &&
								    CoverIce(chunk, biomes, location))
									chunk.SetBlockID(location + Coordinates3D.Up, SnowfallBlock.BlockId);
								else if (!chunk.GetBlockID(location).Equals(SnowfallBlock.BlockId) &&
								         !chunk.GetBlockID(location).Equals(AirBlock.BlockId))
									chunk.SetBlockID(location + Coordinates3D.Up, SnowfallBlock.BlockId);
							}
						}
					}
				}
			}
		}

		private bool CoverIce(IChunk chunk, IBiomeRepository biomes, Coordinates3D location)
		{
			const int maxDistance = 4;
			var adjacent = new[]
			{
				location + new Coordinates3D(-maxDistance, 0, 0),
				location + new Coordinates3D(maxDistance, 0, 0),
				location + new Coordinates3D(0, 0, maxDistance),
				location + new Coordinates3D(0, 0, -maxDistance)
			};
			for (var i = 0; i < adjacent.Length; i++)
			{
				var check = adjacent[i];
				if (check.X < 0 || check.X >= Chunk.Width || check.Z < 0 || check.Z >= Chunk.Depth || check.Y < 0 ||
				    check.Y >= Chunk.Height)
					return false;
				var biome = biomes.GetBiome(chunk.Biomes[check.X * Chunk.Width + check.Z]);
				if (chunk.GetBlockID(check).Equals(biome.SurfaceBlock) ||
				    chunk.GetBlockID(check).Equals(biome.FillerBlock))
					return true;
			}

			return false;
		}
	}
}