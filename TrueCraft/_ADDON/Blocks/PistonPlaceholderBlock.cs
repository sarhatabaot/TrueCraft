using TrueCraft.Logic;

namespace TrueCraft._ADDON.Blocks
{
	public class PistonPlaceholderBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x24;

		public override byte Id => 0x24;

		public override double BlastResistance => 0;

		public override double Hardness => 0;

		public override byte Luminance => 0;

		public override string DisplayName => "Piston Placeholder";
	}
}