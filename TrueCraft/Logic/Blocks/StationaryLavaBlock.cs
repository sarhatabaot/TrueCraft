namespace TrueCraft.Logic.Blocks
{
	public class StationaryLavaBlock : LavaBlock
	{
		public new static readonly byte BlockID = 0x0B;

		public override byte ID => 0x0B;

		public override double BlastResistance => 500;

		public override string DisplayName => "Lava (stationary)";
	}
}