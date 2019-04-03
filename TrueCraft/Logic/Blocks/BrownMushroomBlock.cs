using System;

namespace TrueCraft.Logic.Blocks
{
	public class BrownMushroomBlock : MushroomBlock
	{
		public static readonly byte BlockID = 0x27;

		public override byte ID => 0x27;

		public override byte Luminance => 1;

		public override bool Opaque => false;

		public override string DisplayName => "Brown Mushroom";

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(13, 1);
		}
	}
}