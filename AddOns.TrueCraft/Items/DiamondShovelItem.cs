using System;

namespace TrueCraft.Items
{
	public class DiamondShovelItem : ShovelItem
	{
		public static readonly short ItemId = 0x115;

		public override short Id => 0x115;

		public override ToolMaterial Material => ToolMaterial.Diamond;

		public override short BaseDurability => 1562;

		public override string DisplayName => "Diamond Shovel";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(3, 5);
		}
	}
}