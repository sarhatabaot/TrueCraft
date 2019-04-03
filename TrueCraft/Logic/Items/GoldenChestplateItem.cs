using System;

namespace TrueCraft.Logic.Items
{
	public class GoldenChestplateItem : ChestplateItem
	{
		public static readonly short ItemID = 0x13B;

		public override short ID => 0x13B;

		public override ArmorMaterial Material => ArmorMaterial.Gold;

		public override short BaseDurability => 96;

		public override float BaseArmor => 4;

		public override string DisplayName => "Golden Chestplate";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(4, 1);
		}
	}
}