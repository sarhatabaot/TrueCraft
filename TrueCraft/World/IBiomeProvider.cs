namespace TrueCraft.World
{
	public interface IBiomeProvider
	{
		bool Spawn { get; }
		byte Id { get; }
		int Elevation { get; }
		double Temperature { get; }
		double Rainfall { get; }
		TreeSpecies[] Trees { get; }
		PlantSpecies[] Plants { get; }
		OreTypes[] Ores { get; }
		double TreeDensity { get; }
		byte WaterBlock { get; }
		byte SurfaceBlock { get; }
		byte FillerBlock { get; }
		int SurfaceDepth { get; }
		int FillerDepth { get; }
	}
}