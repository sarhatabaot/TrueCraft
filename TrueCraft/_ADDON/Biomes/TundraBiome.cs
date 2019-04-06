using TrueCraft._ADDON.Blocks;

namespace TrueCraft._ADDON.Biomes
{
	public class TundraBiome : BiomeProvider
	{
		public override byte Id => (byte) Biome.Tundra;

		public override double Temperature => 0.1f;

		public override double Rainfall => 0.7f;

		public override TreeSpecies[] Trees => new[] {TreeSpecies.Spruce};

		public override PlantSpecies[] Plants => new PlantSpecies[0];

		public override double TreeDensity => 50;

		public override byte SurfaceBlock => GrassBlock.BlockId;

		public override byte FillerBlock => DirtBlock.BlockId;
	}
}