using System;
using TrueCraft.Logic.Items;

namespace TrueCraft.Items
{
	public class AppleItem : FoodItem
	{
		public static readonly short ItemId = 0x104;

		public override short Id => 0x104;

		public override float Restores => 2;

		public override string DisplayName => "Apple";

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(10, 0);
		}
	}
}