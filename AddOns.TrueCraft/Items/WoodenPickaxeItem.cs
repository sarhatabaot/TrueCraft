﻿using System;

namespace TrueCraft.Items
{
	public class WoodenPickaxeItem : PickaxeItem
	{
		public static readonly short ItemId = 0x10E;

		public override short Id => 0x10E;

		public override ToolMaterial Material => ToolMaterial.Wood;

		public override short BaseDurability => 60;

		public override string DisplayName => "Wooden Pickaxe";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(0, 6);
		}
	}
}