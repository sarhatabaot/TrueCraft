using System;

namespace TrueCraft.Items
{
	public class LeatherTunicItem : ChestplateItem
	{
		public static readonly short ItemId = 0x12B;

		public override short Id => 0x12B;

		public override ArmorMaterial Material => ArmorMaterial.Leather;

		public override short BaseDurability => 49;

		public override float BaseArmor => 4;

		public override string DisplayName => "Leather Tunic";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(0, 1);
		}
	}
}