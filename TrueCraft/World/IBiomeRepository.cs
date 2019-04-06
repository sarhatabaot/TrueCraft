namespace TrueCraft.World
{
	public interface IBiomeRepository
	{
		IBiomeProvider GetBiome(byte Id);
		IBiomeProvider GetBiome(double temperature, double rainfall, bool spawn);
		void RegisterBiomeProvider(IBiomeProvider provider);
	}
}