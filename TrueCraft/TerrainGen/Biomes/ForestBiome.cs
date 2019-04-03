﻿namespace TrueCraft.TerrainGen.Biomes
{
	public class ForestBiome : BiomeProvider
	{
		public override byte ID => (byte) Biome.Forest;

		public override double Temperature => 0.7f;

		public override double Rainfall => 0.8f;

		public override PlantSpecies[] Plants => new[] {PlantSpecies.TallGrass};
	}
}