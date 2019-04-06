using System;

namespace TrueCraft.Items
{
	public class WoodenAxeItem : AxeItem
	{
		public static readonly short ItemId = 0x10F;

		public override short Id => 0x10F;

		public override ToolMaterial Material => ToolMaterial.Wood;

		public override short BaseDurability => 60;

		public override string DisplayName => "Wooden Axe";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(0, 7);
		}
	}
}