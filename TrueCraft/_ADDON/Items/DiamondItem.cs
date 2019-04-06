using System;
using TrueCraft.Logic;

namespace TrueCraft._ADDON.Items
{
	public class DiamondItem : ItemProvider
	{
		public static readonly short ItemId = 0x108;

		public override short Id => 0x108;

		public override string DisplayName => "Diamond";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(7, 3);
		}
	}
}