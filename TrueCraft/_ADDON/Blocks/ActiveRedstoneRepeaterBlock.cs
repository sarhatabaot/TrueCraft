namespace TrueCraft.Logic.Blocks
{
	public class ActiveRedstoneRepeaterBlock : RedstoneRepeaterBlock
	{
		public new static readonly byte BlockId = 0x5E;

		public override byte Id => 0x5E;

		public override string DisplayName => "Redstone Repeater (active)";
	}
}