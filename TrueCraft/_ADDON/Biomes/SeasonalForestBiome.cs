namespace TrueCraft.TerrainGen.Biomes
{
	public class SeasonalForestBiome : BiomeProvider
	{
		public override byte Id => (byte) Biome.SeasonalForest;

		public override double Temperature => 0.7f;

		public override double Rainfall => 0.8f;

		public override PlantSpecies[] Plants => new[] {PlantSpecies.Fern, PlantSpecies.TallGrass};
	}
}