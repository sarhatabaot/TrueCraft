using System;
using TrueCraft.Logic;

namespace TrueCraft._ADDON.Items
{
	public class BrickItem : ItemProvider
	{
		public static readonly short ItemId = 0x150;

		public override short Id => 0x150;

		public override string DisplayName => "Brick";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(6, 1);
		}
	}
}