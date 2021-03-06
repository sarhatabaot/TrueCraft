﻿using System;
using TrueCraft._ADDON.Items;

namespace TrueCraft.Items
{
	public class ChainLeggingsItem : ArmorItem // Not HelmentItem because it can't inherit the recipe
	{
		public static readonly short ItemId = 0x130;

		public override short Id => 0x130;

		public override ArmorMaterial Material => ArmorMaterial.Chain;

		public override short BaseDurability => 92;

		public override float BaseArmor => 3;

		public override string DisplayName => "Chain Leggings";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(1, 2);
		}
	}
}