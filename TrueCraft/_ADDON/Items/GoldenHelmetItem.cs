using System;

namespace TrueCraft.Logic.Items
{
	public class GoldenHelmetItem : HelmentItem
	{
		public static readonly short ItemID = 0x13A;

		public override short Id => 0x13A;

		public override ArmorMaterial Material => ArmorMaterial.Gold;

		public override short BaseDurability => 68;

		public override float BaseArmor => 1.5f;

		public override string DisplayName => "Golden Helmet";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(4, 0);
		}
	}
}