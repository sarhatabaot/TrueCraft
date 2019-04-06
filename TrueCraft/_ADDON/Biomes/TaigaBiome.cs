namespace TrueCraft.TerrainGen.Biomes
{
	public class TaigaBiome : BiomeProvider
	{
		public override byte Id => (byte) Biome.Taiga;

		public override double Temperature => 0.0f;

		public override double Rainfall => 0.0f;

		public override TreeSpecies[] Trees => new[] {TreeSpecies.Spruce};

		public override double TreeDensity => 5;
	}
}