using System;

namespace TrueCraft._ADDON.Items
{
	public class CookedPorkchopItem : FoodItem
	{
		public static readonly short ItemId = 0x140;

		public override short Id => 0x140;

		public override float Restores => 4;

		public override string DisplayName => "Cooked Porkchop";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(8, 5);
		}
	}
}