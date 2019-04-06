using System;

namespace TrueCraft.Logic.Items
{
	public class DiamondHoeItem : HoeItem
	{
		public static readonly short ItemID = 0x125;

		public override short Id => 0x125;

		public override ToolMaterial Material => ToolMaterial.Diamond;

		public override short BaseDurability => 1562;

		public override string DisplayName => "Diamond Hoe";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(3, 8);
		}
	}
}