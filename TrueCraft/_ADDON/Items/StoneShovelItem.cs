using System;

namespace TrueCraft.Logic.Items
{
	public class StoneShovelItem : ShovelItem
	{
		public static readonly short ItemId = 0x111;

		public override short Id => 0x111;

		public override ToolMaterial Material => ToolMaterial.Stone;

		public override short BaseDurability => 132;

		public override string DisplayName => "Stone Shovel";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(1, 5);
		}
	}
}