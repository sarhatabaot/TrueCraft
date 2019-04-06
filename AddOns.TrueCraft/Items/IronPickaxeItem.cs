using System;

namespace TrueCraft.Items
{
	public class IronPickaxeItem : PickaxeItem
	{
		public static readonly short ItemId = 0x101;

		public override short Id => 0x101;

		public override ToolMaterial Material => ToolMaterial.Iron;

		public override short BaseDurability => 251;

		public override string DisplayName => "Iron Pickaxe";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(2, 6);
		}
	}
}