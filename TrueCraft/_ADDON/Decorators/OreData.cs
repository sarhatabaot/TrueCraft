namespace TrueCraft.TerrainGen.Decorators
{
	public struct OreData
	{
		public byte Id;
		public OreTypes Type;
		public int MinY;
		public int MaxY;
		public int Veins;
		public int Abundance;
		public float Rarity;

		public OreData(byte id, OreTypes type, int minY, int maxY, int viens, int abundance, float rarity)
		{
			Id = id;
			Type = type;
			MinY = minY;
			MaxY = maxY;
			Veins = viens;
			Abundance = abundance;
			Rarity = rarity;
		}
	}
}