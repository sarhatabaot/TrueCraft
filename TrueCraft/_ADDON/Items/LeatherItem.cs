using System;

namespace TrueCraft.Logic.Items
{
	public class LeatherItem : ItemProvider
	{
		public static readonly short ItemId = 0x14E;

		public override short Id => 0x14E;

		public override string DisplayName => "Leather";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(7, 6);
		}
	}
}