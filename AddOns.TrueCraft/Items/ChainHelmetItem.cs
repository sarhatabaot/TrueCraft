﻿using System;
using TrueCraft._ADDON.Items;

namespace TrueCraft.Items
{
	public class ChainHelmetItem : ArmorItem // Not HelmentItem because it can't inherit the recipe
	{
		public static readonly short ItemId = 0x12E;

		public override short Id => 0x12E;

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