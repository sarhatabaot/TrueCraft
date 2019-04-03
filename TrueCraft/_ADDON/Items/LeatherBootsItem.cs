using System;

namespace TrueCraft.Logic.Items
{
	public class LeatherBootsItem : BootsItem
	{
		public static readonly short ItemID = 0x12D;

		public override short ID => 0x12D;

		public override ArmorMaterial Material => ArmorMaterial.Leather;

		public override short BaseDurability => 40;

		public override float BaseArmor => 1.5f;

		public override string DisplayName => "Leather Boots";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(0, 3);
		}
	}
}