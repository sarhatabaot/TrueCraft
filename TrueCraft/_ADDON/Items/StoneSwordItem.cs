using System;

namespace TrueCraft.Logic.Items
{
	public class StoneSwordItem : SwordItem
	{
		public static readonly short ItemID = 0x110;

		public override short Id => 0x110;

		public override ToolMaterial Material => ToolMaterial.Stone;

		public override short BaseDurability => 132;

		public override float Damage => 3.5f;

		public override string DisplayName => "Stone Sword";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(1, 4);
		}
	}
}