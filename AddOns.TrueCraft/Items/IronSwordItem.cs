using System;

namespace TrueCraft.Items
{
	public class IronSwordItem : SwordItem
	{
		public static readonly short ItemId = 0x10B;

		public override short Id => 0x10B;

		public override ToolMaterial Material => ToolMaterial.Iron;

		public override short BaseDurability => 251;

		public override float Damage => 4.5f;

		public override string DisplayName => "Iron Sword";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(2, 4);
		}
	}
}