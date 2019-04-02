using System;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
	public class ChainHelmetItem : ArmorItem // Not HelmentItem because it can't inherit the recipe
	{
		public static readonly short ItemID = 0x12E;

		public override short ID => 0x12E;

		public override ArmorMaterial Material => ArmorMaterial.Chain;

		public override short BaseDurability => 67;

		public override float BaseArmor => 1.5f;

		public override string DisplayName => "Chain Helmet";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(1, 0);
		}
	}
}