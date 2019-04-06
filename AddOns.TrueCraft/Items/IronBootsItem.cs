using System;

namespace TrueCraft.Items
{
	public class IronBootsItem : BootsItem
	{
		public static readonly short ItemId = 0x135;

		public override short Id => 0x135;

		public override ArmorMaterial Material => ArmorMaterial.Iron;

		public override short BaseDurability => 160;

		public override float BaseArmor => 1.5f;

		public override string DisplayName => "Iron Boots";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(2, 3);
		}
	}
}