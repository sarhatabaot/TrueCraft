using System;
using TrueCraft._ADDON.Items;

namespace TrueCraft.Items
{
	public class ChainChestplateItem : ArmorItem // Not HelmentItem because it can't inherit the recipe
	{
		public static readonly short ItemId = 0x12F;

		public override short Id => 0x12F;

		public override ArmorMaterial Material => ArmorMaterial.Chain;

		public override short BaseDurability => 96;

		public override float BaseArmor => 4;

		public override string DisplayName => "Chain Chestplate";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(1, 1);
		}
	}
}