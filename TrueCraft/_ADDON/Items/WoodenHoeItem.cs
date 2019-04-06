using System;

namespace TrueCraft.Logic.Items
{
	public class WoodenHoeItem : HoeItem
	{
		public static readonly short ItemId = 0x122;

		public override short Id => 0x122;

		public override ToolMaterial Material => ToolMaterial.Wood;

		public override short BaseDurability => 60;

		public override string DisplayName => "Wooden Hoe";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(0, 8);
		}
	}
}