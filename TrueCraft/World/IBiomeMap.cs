using System.Collections.Generic;

namespace TrueCraft.World
{
	public interface IBiomeMap
	{
		IList<BiomeCell> BiomeCells { get; }
		void AddCell(BiomeCell cell);
		byte GetBiome(Coordinates2D location);
		byte GenerateBiome(int seed, IBiomeRepository biomes, Coordinates2D location, bool spawn);
		BiomeCell ClosestCell(Coordinates2D location);
		double ClosestCellPoint(Coordinates2D location);
	}
}