using System;

namespace TrueCraft._ADDON.Blocks
{
	public class InactiveRedstoneTorchBlock : RedstoneTorchBlock
	{
		public new static readonly byte BlockId = 0x4B;

		public override byte Id => 0x4B;

		public override byte Luminance => 0;

		public override string DisplayName => "Redstone Torch (inactive)";

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(3, 7);
		}
	}
}