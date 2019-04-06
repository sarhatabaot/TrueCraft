using System;

namespace TrueCraft.Items
{
	public class DiamondSwordItem : SwordItem
	{
		public static readonly short ItemId = 0x114;

		public override short Id => 0x114;

		public override ToolMaterial Material => ToolMaterial.Diamond;

		public override short BaseDurability => 1562;

		public override float Damage => 5.5f;

		public override string DisplayName => "Diamond Sword";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(3, 4);
		}
	}
}