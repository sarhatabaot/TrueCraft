using System;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
	public class IronHoeItem : HoeItem
	{
		public static readonly short ItemID = 0x124;

		public override short ID => 0x124;

		public override ToolMaterial Material => ToolMaterial.Iron;

		public override short BaseDurability => 251;

		public override string DisplayName => "Iron Hoe";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(2, 8);
		}
	}
}