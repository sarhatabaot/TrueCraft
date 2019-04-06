namespace TrueCraft.TerrainGen.Biomes
{
	public class SavannaBiome : BiomeProvider
	{
		public override byte Id => (byte) Biome.Savanna;

		public override double Temperature => 1.2f;

		public override double Rainfall => 0.0f;

		public override TreeSpecies[] Trees => new[] {TreeSpecies.Oak};

		public override PlantSpecies[] Plants => new[] {PlantSpecies.Deadbush, PlantSpecies.Fern};

		public override double TreeDensity => 50;
	}
}