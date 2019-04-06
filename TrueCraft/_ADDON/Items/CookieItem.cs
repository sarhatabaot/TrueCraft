using System;
using TrueCraft.Logic;

namespace TrueCraft._ADDON.Items
{
	public class CookieItem : FoodItem, ICraftingRecipe
	{
		public static readonly short ItemId = 0x165;

		public override short Id => 0x165;

		public override sbyte MaximumStack => 8;

		public override float Restores => 0.5f;

		public override string DisplayName => "Cookie";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(WheatItem.ItemId),
					new ItemStack(DyeItem.ItemId, 1, (short) DyeItem.DyeType.CocoaBeans),
					new ItemStack(WheatItem.ItemId)
				}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => true;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(12, 5);
		}
	}
}