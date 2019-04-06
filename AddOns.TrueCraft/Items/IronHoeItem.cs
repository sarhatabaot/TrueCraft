using System;

namespace TrueCraft.Items
{
	public class IronHoeItem : HoeItem
	{
		public static readonly short ItemId = 0x124;

		public override short Id => 0x124;

		public override ToolMaterial Material => ToolMaterial.Iron;

		public override short BaseDurability => 251;

		public override string DisplayName => "Iron Hoe";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(2, 8);
		}
	}
}