using System;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Logic.Items
{
	public class WoodenDoorItem : DoorItem
	{
		public static readonly short ItemID = 0x144;

		public override short ID => 0x144;

		public override string DisplayName => "Wooden Door";

		protected override byte BlockID => WoodenDoorBlock.BlockID;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(11, 2);
		}
	}
}