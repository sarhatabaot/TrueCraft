using System;

namespace TrueCraft.Logic.Blocks
{
	public class InactiveRedstoneTorchBlock : RedstoneTorchBlock
	{
		public new static readonly byte BlockID = 0x4B;

		public override byte ID => 0x4B;

		public override byte Luminance => 0;

		public override string DisplayName => "Redstone Torch (inactive)";

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(3, 7);
		}
	}
}