using System;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
	public class LeatherPantsItem : LeggingsItem
	{
		public static readonly short ItemID = 0x12C;

		public override short ID => 0x12C;

		public override ArmorMaterial Material => ArmorMaterial.Leather;

		public override short BaseDurability => 46;

		public override float BaseArmor => 3;

		public override string DisplayName => "Leather Pants";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(0, 2);
		}
	}
}