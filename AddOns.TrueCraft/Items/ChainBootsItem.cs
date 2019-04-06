using System;
using TrueCraft.Logic.Items;

namespace TrueCraft.Items
{
	public class ChainBootsItem : ArmorItem // Not HelmentItem because it can't inherit the recipe
	{
		public static readonly short ItemId = 0x131;

		public override short Id => 0x131;

		public override ArmorMaterial Material => ArmorMaterial.Chain;

		public override short BaseDurability => 79;

		public override float BaseArmor => 1.5f;

		public override string DisplayName => "Chain Boots";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(1, 3);
		}
	}
}