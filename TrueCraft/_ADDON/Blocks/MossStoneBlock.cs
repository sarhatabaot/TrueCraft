using System;
using TrueCraft.Logic;

namespace TrueCraft._ADDON.Blocks
{
	public class MossStoneBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x30;

		public override byte Id => 0x30;

		public override double BlastResistance => 30;

		public override double Hardness => 2;

		public override byte Luminance => 0;

		public override string DisplayName => "Moss Stone";

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(4, 2);
		}
	}
}