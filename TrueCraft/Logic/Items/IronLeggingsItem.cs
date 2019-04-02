﻿using System;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
	public class IronLeggingsItem : LeggingsItem
	{
		public static readonly short ItemID = 0x134;

		public override short ID => 0x134;

		public override ArmorMaterial Material => ArmorMaterial.Iron;

		public override short BaseDurability => 184;

		public override float BaseArmor => 3;

		public override string DisplayName => "Iron Leggings";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(2, 2);
		}
	}
}