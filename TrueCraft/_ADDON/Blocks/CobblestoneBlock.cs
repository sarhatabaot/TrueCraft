using System;
using TrueCraft.Logic;

namespace TrueCraft._ADDON.Blocks
{
	public class CobblestoneBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x04;

		public override byte Id => 0x04;

		public override double BlastResistance => 30;

		public override double Hardness => 2;

		public override byte Luminance => 0;

		public override string DisplayName => "Cobblestone";

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(0, 1);
		}
	}
}