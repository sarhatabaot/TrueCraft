using System;

namespace TrueCraft.Logic.Items
{
	public class GoldenLeggingsItem : LeggingsItem
	{
		public static readonly short ItemId = 0x13C;

		public override short Id => 0x13C;

		public override ArmorMaterial Material => ArmorMaterial.Gold;

		public override short BaseDurability => 92;

		public override float BaseArmor => 3;

		public override string DisplayName => "Golden Leggings";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(4, 2);
		}
	}
}