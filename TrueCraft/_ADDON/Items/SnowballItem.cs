using System;
using TrueCraft.Logic;

namespace TrueCraft._ADDON.Items
{
	public class SnowballItem : ItemProvider
	{
		public static readonly short ItemId = 0x14C;

		public override short Id => 0x14C;

		public override sbyte MaximumStack => 16;

		public override string DisplayName => "Snowball";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(14, 0);
		}
	}
}