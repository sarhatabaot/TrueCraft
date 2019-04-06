using System;

namespace TrueCraft._ADDON.Blocks
{
	public class BrownMushroomBlock : MushroomBlock
	{
		public static readonly byte BlockId = 0x27;

		public override byte Id => 0x27;

		public override byte Luminance => 1;

		public override bool Opaque => false;

		public override string DisplayName => "Brown Mushroom";

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(13, 1);
		}
	}
}