using System;
using TrueCraft.Logic;

namespace TrueCraft._ADDON.Items
{
	public class SlimeballItem : ItemProvider
	{
		public static readonly short ItemId = 0x155;

		public override short Id => 0x155;

		public override string DisplayName => "Slimeball";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(14, 1);
		}
	}
}