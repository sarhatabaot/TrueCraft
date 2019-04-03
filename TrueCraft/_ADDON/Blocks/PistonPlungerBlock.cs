using System;

namespace TrueCraft.Logic.Blocks
{
	public class PistonPlungerBlock : BlockProvider
	{
		public static readonly byte BlockID = 0x22;

		public override byte ID => 0x22;

		public override double BlastResistance => 2.5;

		public override double Hardness => 0.5;

		public override byte Luminance => 0;

		public override string DisplayName => "Piston Plunger";

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(11, 6);
		}
	}
}