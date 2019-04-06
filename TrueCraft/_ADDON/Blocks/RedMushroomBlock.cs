using System;

namespace TrueCraft._ADDON.Blocks
{
	public class RedMushroomBlock : MushroomBlock
	{
		public static readonly byte BlockId = 0x28;

		public override byte Id => 0x28;

		public override byte Luminance => 0;

		public override string DisplayName => "Red Mushroom";

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(12, 1);
		}
	}
}