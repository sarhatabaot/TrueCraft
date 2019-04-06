using System;
using TrueCraft.Logic;

namespace TrueCraft._ADDON.Items
{
	public class StringItem : ItemProvider
	{
		public static readonly short ItemId = 0x11F;

		public override short Id => 0x11F;

		public override string DisplayName => "String";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(8, 0);
		}
	}
}