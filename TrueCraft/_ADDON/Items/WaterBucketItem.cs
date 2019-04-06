using TrueCraft._ADDON.Blocks;

namespace TrueCraft._ADDON.Items
{
	public class WaterBucketItem : BucketItem
	{
		public new static readonly short ItemId = 0x146;

		public override short Id => 0x146;

		public override string DisplayName => "Water Bucket";

		protected override byte? RelevantBlockType => WaterBlock.BlockId;
	}
}