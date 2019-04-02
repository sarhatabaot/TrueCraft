using System;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
	public class ChainChestplateItem : ArmorItem // Not HelmentItem because it can't inherit the recipe
	{
		public static readonly short ItemID = 0x12F;

		public override short ID => 0x12F;

		public override ArmorMaterial Material => ArmorMaterial.Chain;

		public override short BaseDurability => 96;

		public override float BaseArmor => 4;

		public override string DisplayName => "Chain Chestplate";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(1, 1);
		}
	}
}