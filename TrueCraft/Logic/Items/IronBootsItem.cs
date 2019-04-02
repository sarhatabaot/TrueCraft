﻿using System;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
	public class IronBootsItem : BootsItem
	{
		public static readonly short ItemID = 0x135;

		public override short ID => 0x135;

		public override ArmorMaterial Material => ArmorMaterial.Iron;

		public override short BaseDurability => 160;

		public override float BaseArmor => 1.5f;

		public override string DisplayName => "Iron Boots";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(2, 3);
		}
	}
}