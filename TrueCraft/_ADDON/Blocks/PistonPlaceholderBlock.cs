namespace TrueCraft.Logic.Blocks
{
	public class PistonPlaceholderBlock : BlockProvider
	{
		public static readonly byte BlockID = 0x24;

		public override byte ID => 0x24;

		public override double BlastResistance => 0;

		public override double Hardness => 0;

		public override byte Luminance => 0;

		public override string DisplayName => "Piston Placeholder";
	}
}