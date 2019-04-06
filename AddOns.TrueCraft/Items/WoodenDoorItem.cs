using System;
using TrueCraft.Blocks;

namespace TrueCraft.Items
{
	public class WoodenDoorItem : DoorItem
	{
		public static readonly short ItemId = 0x144;

		public override short Id => 0x144;

		public override string DisplayName => "Wooden Door";

		protected override byte BlockId => WoodenDoorBlock.BlockId;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(11, 2);
		}
	}
}