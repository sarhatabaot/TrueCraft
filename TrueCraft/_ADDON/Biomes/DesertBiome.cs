using TrueCraft._ADDON.Blocks;

namespace TrueCraft._ADDON.Biomes
{
	public class DesertBiome : BiomeProvider
	{
		public override byte Id => (byte) Biome.Desert;

		public override double Temperature => 2.0f;

		public override double Rainfall => 0.0f;

		public override bool Spawn => false;

		public override TreeSpecies[] Trees => new TreeSpecies[0];

		public override PlantSpecies[] Plants => new[] {PlantSpecies.Deadbush, PlantSpecies.Cactus};

		public override byte SurfaceBlock => SandBlock.BlockId;

		public override byte FillerBlock => SandstoneBlock.BlockId;

		public override int SurfaceDepth => 4;
	}
}