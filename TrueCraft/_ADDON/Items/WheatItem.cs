using System;

namespace TrueCraft.Logic.Items
{
	public class WheatItem : ItemProvider
	{
		public static readonly short ItemID = 0x128;

		public override short Id => 0x128;

		public override string DisplayName => "Wheat";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(9, 1);
		}
	}
}