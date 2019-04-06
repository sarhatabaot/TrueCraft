using System;

namespace TrueCraft.Logic.Items
{
	public class IronChestplateItem : ChestplateItem
	{
		public static readonly short ItemID = 0x133;

		public override short Id => 0x133;

		public override ArmorMaterial Material => ArmorMaterial.Iron;

		public override short BaseDurability => 192;

		public override float BaseArmor => 4;

		public override string DisplayName => "Iron Chestplate";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(2, 1);
		}
	}
}