using System;

namespace TrueCraft.Core.Logic.Items
{
	public class GlowstoneDustItem : ItemProvider
	{
		public static readonly short ItemID = 0x15C;

		public override short ID => 0x15C;

		public override string DisplayName => "Glowstone Dust";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(9, 4);
		}
	}
}