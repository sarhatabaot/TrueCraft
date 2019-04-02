using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
	public class WaterBucketItem : BucketItem
	{
		public new static readonly short ItemID = 0x146;

		public override short ID => 0x146;

		public override string DisplayName => "Water Bucket";

		protected override byte? RelevantBlockType => WaterBlock.BlockID;
	}
}