using System;

namespace TrueCraft.Logic.Items
{
	public class GoldenHoeItem : HoeItem
	{
		public static readonly short ItemID = 0x126;

		public override short ID => 0x126;

		public override ToolMaterial Material => ToolMaterial.Gold;

		public override short BaseDurability => 33;

		public override string DisplayName => "Golden Hoe";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(4, 8);
		}
	}
}