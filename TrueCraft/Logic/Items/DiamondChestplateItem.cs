using System;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
	public class DiamondChestplateItem : ChestplateItem
	{
		public static readonly short ItemID = 0x137;

		public override short ID => 0x137;

		public override ArmorMaterial Material => ArmorMaterial.Diamond;

		public override short BaseDurability => 384;

		public override float BaseArmor => 4;

		public override string DisplayName => "Diamond Chestplate";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(3, 1);
		}
	}
}