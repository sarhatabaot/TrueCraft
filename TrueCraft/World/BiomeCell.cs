namespace TrueCraft.API.World
{
	public class BiomeCell
	{
		public byte BiomeID;
		public Coordinates2D CellPoint;

		public BiomeCell(byte biomeID, Coordinates2D cellPoint)
		{
			BiomeID = biomeID;
			CellPoint = cellPoint;
		}
	}
}