namespace TrueCraft._ADDON.Biomes
{
	public class RainforestBiome : BiomeProvider
	{
		public override byte Id => (byte) Biome.Rainforest;

		public override double Temperature => 1.2f;

		public override double Rainfall => 0.9f;

		public override TreeSpecies[] Trees => new[] {TreeSpecies.Birch, TreeSpecies.Oak};

		public override double TreeDensity => 2;

		public override PlantSpecies[] Plants => new[] {PlantSpecies.Fern, PlantSpecies.TallGrass};
	}
}