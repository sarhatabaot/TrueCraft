using System;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
	public class DiamondAxeItem : AxeItem
	{
		public static readonly short ItemID = 0x117;

		public override short ID => 0x117;

		public override ToolMaterial Material => ToolMaterial.Diamond;

		public override short BaseDurability => 1562;

		public override string DisplayName => "Diamond Axe";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(3, 7);
		}
	}
}