namespace TrueCraft._ADDON.Items
{
	public class MilkItem : BucketItem
	{
		public new static readonly short ItemId = 0x14F;

		public override short Id => 0x14F;

		public override string DisplayName => "Milk";

		protected override byte? RelevantBlockType => null;
	}
}