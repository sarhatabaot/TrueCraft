﻿using System;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
	public class WoodenSwordItem : SwordItem
	{
		public static readonly short ItemID = 0x10C;

		public override short ID => 0x10C;

		public override ToolMaterial Material => ToolMaterial.Wood;

		public override short BaseDurability => 60;

		public override float Damage => 2.5f;

		public override string DisplayName => "Wooden Sword";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(0, 4);
		}
	}
}