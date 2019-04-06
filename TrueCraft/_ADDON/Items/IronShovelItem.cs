using System;

namespace TrueCraft.Logic.Items
{
	public class IronShovelItem : ShovelItem
	{
		public static readonly short ItemId = 0x100;

		public override short Id => 0x100;

		public override ToolMaterial Material => ToolMaterial.Iron;

		public override short BaseDurability => 251;

		public override string DisplayName => "Iron Shovel";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(2, 5);
		}
	}
}