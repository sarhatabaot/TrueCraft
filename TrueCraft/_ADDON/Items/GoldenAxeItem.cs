using System;

namespace TrueCraft.Logic.Items
{
	public class GoldenAxeItem : AxeItem
	{
		public static readonly short ItemID = 0x11E;

		public override short ID => 0x11E;

		public override ToolMaterial Material => ToolMaterial.Gold;

		public override short BaseDurability => 33;

		public override string DisplayName => "Golden Axe";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(4, 7);
		}
	}
}