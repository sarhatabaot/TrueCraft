using System;

namespace TrueCraft.Items
{
	public class WoodenShovelItem : ShovelItem
	{
		public static readonly short ItemId = 0x10D;

		public override short Id => 0x10D;

		public override ToolMaterial Material => ToolMaterial.Wood;

		public override short BaseDurability => 60;

		public override string DisplayName => "Wooden Shovel";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(0, 5);
		}
	}
}