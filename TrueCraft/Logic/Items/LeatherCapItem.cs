﻿using System;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
	public class LeatherCapItem : HelmentItem
	{
		public static readonly short ItemID = 0x12A;

		public override short ID => 0x12A;

		public override ArmorMaterial Material => ArmorMaterial.Leather;

		public override short BaseDurability => 34;

		public override float BaseArmor => 1.5f;

		public override string DisplayName => "Leather Cap";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(0, 0);
		}
	}
}