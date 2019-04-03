namespace TrueCraft.Logic.Blocks
{
	public class ActiveRedstoneRepeaterBlock : RedstoneRepeaterBlock
	{
		public new static readonly byte BlockID = 0x5E;

		public override byte ID => 0x5E;

		public override string DisplayName => "Redstone Repeater (active)";
	}
}