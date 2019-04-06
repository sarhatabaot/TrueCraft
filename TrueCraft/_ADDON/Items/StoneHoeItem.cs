using System;

namespace TrueCraft.Logic.Items
{
	public class StoneHoeItem : HoeItem
	{
		public static readonly short ItemID = 0x123;

		public override short Id => 0x123;

		public override ToolMaterial Material => ToolMaterial.Stone;

		public override short BaseDurability => 132;

		public override string DisplayName => "Stone Hoe";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(1, 8);
		}
	}
}