using System;
using TrueCraft.API;

namespace TrueCraft.Core.Logic.Items
{
	public class GoldenSwordItem : SwordItem
	{
		public static readonly short ItemID = 0x11B;

		public override short ID => 0x11B;

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