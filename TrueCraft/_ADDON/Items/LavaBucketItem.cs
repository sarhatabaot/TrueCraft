using System;
using TrueCraft.Logic;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft._ADDON.Items
{
	public class LavaBucketItem : BucketItem, IBurnableItem
	{
		public new static readonly short ItemId = 0x147;

		public override short Id => 0x147;

		public override string DisplayName => "Lava Bucket";

		protected override byte? RelevantBlockType => LavaBlock.BlockId;

		public TimeSpan BurnTime => TimeSpan.FromSeconds(1000);
	}
}