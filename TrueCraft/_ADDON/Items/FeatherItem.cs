using System;

namespace TrueCraft.Logic.Items
{
	public class FeatherItem : ItemProvider
	{
		public static readonly short ItemID = 0x120;

		public override short Id => 0x120;

		public override string DisplayName => "Feather";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(8, 1);
		}
	}
}