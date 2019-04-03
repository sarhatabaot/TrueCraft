using System;

namespace TrueCraft.Logic.Blocks
{
	public class RedMushroomBlock : MushroomBlock
	{
		public static readonly byte BlockID = 0x28;

		public override byte ID => 0x28;

		public override byte Luminance => 0;

		public override string DisplayName => "Red Mushroom";

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(12, 1);
		}
	}
}