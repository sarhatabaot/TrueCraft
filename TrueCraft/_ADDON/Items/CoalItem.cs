using System;

namespace TrueCraft.Logic.Items
{
	public class CoalItem : ItemProvider, IBurnableItem
	{
		public static readonly short ItemId = 0x107;

		public override short Id => 0x107;

		public override string DisplayName => "Coal";

		public TimeSpan BurnTime => TimeSpan.FromSeconds(80);

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(7, 0);
		}
	}
}