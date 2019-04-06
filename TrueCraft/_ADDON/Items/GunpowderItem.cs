using System;
using TrueCraft.Logic;

namespace TrueCraft._ADDON.Items
{
	public class GunpowderItem : ItemProvider
	{
		public static readonly short ItemId = 0x121;

		public override short Id => 0x121;

		public override string DisplayName => "Gunpowder";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(8, 2);
		}
	}
}