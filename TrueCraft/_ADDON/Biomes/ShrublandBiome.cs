namespace TrueCraft._ADDON.Biomes
{
	public class ShrublandBiome : BiomeProvider
	{
		public override byte Id => (byte) Biome.Shrubland;

		public override double Temperature => 0.8f;

		public override double Rainfall => 0.4f;

		public override TreeSpecies[] Trees => new[] {TreeSpecies.Oak};

		public override PlantSpecies[] Plants => new PlantSpecies[0];
	}
}