using System;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
	public class IronDoorItem : DoorItem
	{
		public static readonly short ItemID = 0x14A;

		public override short ID => 0x14A;

		public override string DisplayName => "Iron Door";

		protected override byte BlockID => IronDoorBlock.BlockID;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(12, 2);
		}
	}
}