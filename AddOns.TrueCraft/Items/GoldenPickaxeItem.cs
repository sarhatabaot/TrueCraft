using System;

namespace TrueCraft.Items
{
	public class GoldenPickaxeItem : PickaxeItem
	{
		public static readonly short ItemId = 0x11D;

		public override short Id => 0x11D;

		public override ToolMaterial Material => ToolMaterial.Gold;

		public override short BaseDurability => 33;

		public override string DisplayName => "Golden Pickaxe";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(4, 6);
		}
	}
}