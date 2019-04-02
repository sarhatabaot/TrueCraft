namespace TrueCraft.Core.Logic.Blocks
{
	public class StationaryWaterBlock : WaterBlock
	{
		public new static readonly byte BlockID = 0x09;

		public override byte ID => 0x09;

		public override string DisplayName => "Water (stationary)";
	}
}