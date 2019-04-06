using System;

namespace TrueCraft.Logic.Items
{
	public class ClayItem : ItemProvider
	{
		public static readonly short ItemId = 0x151;

		public override short Id => 0x151;

		public override string DisplayName => "Clay";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(9, 3);
		}
	}
}