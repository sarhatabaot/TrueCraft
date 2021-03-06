﻿using System;

namespace TrueCraft.Items
{
	public class GoldenBootsItem : BootsItem
	{
		public static readonly short ItemId = 0x13D;

		public override short Id => 0x13D;

		public override ArmorMaterial Material => ArmorMaterial.Gold;

		public override short BaseDurability => 80;

		public override float BaseArmor => 1.5f;

		public override string DisplayName => "Golden Boots";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(4, 3);
		}
	}
}