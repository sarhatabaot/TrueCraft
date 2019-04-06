using System;

namespace TrueCraft.Logic.Items
{
	public class DiamondLeggingsItem : LeggingsItem
	{
		public static readonly short ItemId = 0x138;

		public override short Id => 0x138;

		public override ArmorMaterial Material => ArmorMaterial.Diamond;

		public override short BaseDurability => 368;

		public override float BaseArmor => 3;

		public override string DisplayName => "Diamond Leggings";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(3, 2);
		}
	}
}