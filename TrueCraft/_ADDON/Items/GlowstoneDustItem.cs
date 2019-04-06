using System;

namespace TrueCraft.Logic.Items
{
	public class GlowstoneDustItem : ItemProvider
	{
		public static readonly short ItemId = 0x15C;

		public override short Id => 0x15C;

		public override string DisplayName => "Glowstone Dust";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(9, 4);
		}
	}
}