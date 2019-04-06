using System;

namespace TrueCraft.Logic.Items
{
	public class FlintItem : ItemProvider
	{
		public static readonly short ItemId = 0x13E;

		public override short Id => 0x13E;

		public override string DisplayName => "Flint";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(6, 0);
		}
	}
}