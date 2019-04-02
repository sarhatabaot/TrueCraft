using System;
using TrueCraft.API.Logic;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
	public class LavaBucketItem : BucketItem, IBurnableItem
	{
		public new static readonly short ItemID = 0x147;

		public override short ID => 0x147;

		public override string DisplayName => "Lava Bucket";

		protected override byte? RelevantBlockType => LavaBlock.BlockID;

		public TimeSpan BurnTime => TimeSpan.FromSeconds(1000);
	}
}