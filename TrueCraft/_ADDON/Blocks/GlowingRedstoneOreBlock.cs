namespace TrueCraft.Logic.Blocks
{
	public class GlowingRedstoneOreBlock : RedstoneOreBlock
	{
		public new static readonly byte BlockId = 0x4A;

		public override byte Id => 0x4A;

		public override byte Luminance => 9;

		public override bool Opaque => false;

		public override string DisplayName => "Redstone Ore (glowing)";
	}
}