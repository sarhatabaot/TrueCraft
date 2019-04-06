using System;

namespace TrueCraft.Items
{
	public class DiamondBootsItem : BootsItem
	{
		public static readonly short ItemId = 0x139;

		public override short Id => 0x139;

		public override ArmorMaterial Material => ArmorMaterial.Diamond;

		public override short BaseDurability => 320;

		public override float BaseArmor => 1.5f;

		public override string DisplayName => "Diamond Boots";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(3, 3);
		}
	}
}