using System;

namespace TrueCraft.Items
{
	public class GoldenSwordItem : SwordItem
	{
		public static readonly short ItemId = 0x11B;

		public override short Id => 0x11B;

		public override ToolMaterial Material => ToolMaterial.Gold;

		public override short BaseDurability => 33;

		public override float Damage => 2.5f;

		public override string DisplayName => "Golden Sword";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(4, 4);
		}
	}
}