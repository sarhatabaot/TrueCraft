using System;

namespace TrueCraft.Logic.Items
{
	public class DiamondPickaxeItem : PickaxeItem
	{
		public static readonly short ItemID = 0x116;

		public override short ID => 0x116;

		public override ToolMaterial Material => ToolMaterial.Diamond;

		public override short BaseDurability => 1562;

		public override string DisplayName => "Diamond Pickaxe";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(3, 6);
		}
	}
}