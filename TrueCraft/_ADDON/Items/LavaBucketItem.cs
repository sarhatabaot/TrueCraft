using System;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Logic.Items
{
	public class LavaBucketItem : BucketItem, IBurnableItem
	{
		public new static readonly short ItemID = 0x147;

		public override short Id => 0x147;

		public override string DisplayName => "Lava Bucket";

		protected override byte? RelevantBlockType => LavaBlock.BlockId;

		public TimeSpan BurnTime => TimeSpan.FromSeconds(1000);
	}
}