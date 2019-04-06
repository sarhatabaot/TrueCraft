namespace TrueCraft.Logic.Blocks
{
	public class LitFurnaceBlock : FurnaceBlock
	{
		public new static readonly byte BlockId = 0x3E;

		public override byte Id => 0x3E;

		public override byte Luminance => 13;

		public override bool Opaque => false;

		public override string DisplayName => "Furnace (lit)";
	}
}