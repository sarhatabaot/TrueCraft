using System;

namespace TrueCraft.Logic.Items
{
	public class GoldenShovelItem : ShovelItem
	{
		public static readonly short ItemID = 0x11C;

		public override short Id => 0x11C;

		public override ToolMaterial Material => ToolMaterial.Gold;

		public override short BaseDurability => 33;

		public override string DisplayName => "Golden Shovel";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(4, 5);
		}
	}
}