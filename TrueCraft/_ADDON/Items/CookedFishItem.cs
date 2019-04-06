using System;

namespace TrueCraft.Logic.Items
{
	public class CookedFishItem : FoodItem
	{
		public static readonly short ItemId = 0x15E;

		public override short Id => 0x15E;

		public override float Restores => 2.5f;

		public override string DisplayName => "Cooked Fish";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(10, 5);
		}
	}
}