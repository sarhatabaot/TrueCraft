using System;

namespace TrueCraft.Logic.Items
{
	public class StoneAxeItem : AxeItem
	{
		public static readonly short ItemID = 0x113;

		public override short ID => 0x113;

		public override ToolMaterial Material => ToolMaterial.Stone;

		public override short BaseDurability => 132;

		public override string DisplayName => "Stone Axe";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(1, 7);
		}
	}
}