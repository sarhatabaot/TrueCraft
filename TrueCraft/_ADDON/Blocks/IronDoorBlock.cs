using System;
using TrueCraft.Logic.Items;

namespace TrueCraft.Logic.Blocks
{
	public class IronDoorBlock : DoorBlock
	{
		public static readonly byte BlockId = 0x47;

		public override short ItemID => IronDoorItem.ItemID;

		public override byte Id => 0x47;

		public override double BlastResistance => 25;

		public override double Hardness => 5;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Iron Door";

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(1, 6);
		}
	}
}