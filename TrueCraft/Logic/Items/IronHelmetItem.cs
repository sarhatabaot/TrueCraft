﻿using System;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
	public class IronHelmetItem : HelmentItem
	{
		public static readonly short ItemID = 0x132;

		public override short ID => 0x132;

		public override ArmorMaterial Material => ArmorMaterial.Iron;

		public override short BaseDurability => 136;

		public override float BaseArmor => 1.5f;

		public override string DisplayName => "Iron Helmet";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(2, 0);
		}
	}
}