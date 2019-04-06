namespace TrueCraft.TerrainGen.Biomes
{
	public class PlainsBiome : BiomeProvider
	{
		public override byte Id => (byte) Biome.Plains;

		public override double Temperature => 0.8f;

		public override double Rainfall => 0.4f;

		public override TreeSpecies[] Trees => new[] {TreeSpecies.Oak};
	}
}