using System;
using TrueCraft._ADDON.Items;

namespace TrueCraft.Items
{
	public class MapItem : ToolItem
	{
		public static readonly short ItemId = 0x166;

		public override short Id => 0x166;

		public override sbyte MaximumStack => 1;

		public override string DisplayName => "Map";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(12, 3);
		}
	}
}