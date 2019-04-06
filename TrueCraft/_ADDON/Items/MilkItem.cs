namespace TrueCraft.Logic.Items
{
	public class MilkItem : BucketItem
	{
		public new static readonly short ItemID = 0x14F;

		public override short Id => 0x14F;

		public override string DisplayName => "Milk";

		protected override byte? RelevantBlockType => null;
	}
}