using System;

namespace TrueCraft._ADDON.Items
{
	public class RawPorkchopItem : FoodItem
	{
		public static readonly short ItemId = 0x13F;

		public override short Id => 0x13F;

		public override float Restores => 1.5f;

		public override string DisplayName => "Raw Porkchop";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(7, 5);
		}
	}
}