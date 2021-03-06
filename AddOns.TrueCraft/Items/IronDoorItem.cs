﻿using System;
using TrueCraft.Blocks;

namespace TrueCraft.Items
{
	public class IronDoorItem : DoorItem
	{
		public static readonly short ItemId = 0x14A;

		public override short Id => 0x14A;

		public override string DisplayName => "Iron Door";

		protected override byte BlockId => IronDoorBlock.BlockId;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(12, 2);
		}
	}
}