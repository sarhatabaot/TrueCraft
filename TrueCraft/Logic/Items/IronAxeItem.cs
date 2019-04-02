using System;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
	public class IronAxeItem : AxeItem
	{
		public static readonly short ItemID = 0x102;

		public override short ID => 0x102;

		public override ToolMaterial Material => ToolMaterial.Iron;

		public override short BaseDurability => 251;

		public override string DisplayName => "Iron Axe";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(2, 7);
		}
	}
}