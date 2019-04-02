using System;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
	public class DiamondHelmetItem : HelmentItem
	{
		public static readonly short ItemID = 0x136;

		public override short ID => 0x136;

		public override ArmorMaterial Material => ArmorMaterial.Diamond;

		public override short BaseDurability => 272;

		public override float BaseArmor => 1.5f;

		public override string DisplayName => "Diamond Helmet";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(3, 0);
		}
	}
}