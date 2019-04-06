using System;
using TrueCraft.Logic;

namespace TrueCraft._ADDON.Items
{
	public class BreadItem : FoodItem, ICraftingRecipe
	{
		public static readonly short ItemId = 0x129;

		public override short Id => 0x129;

		public override float Restores => 2.5f;

		public override string DisplayName => "Bread";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(WheatItem.ItemId), new ItemStack(WheatItem.ItemId), new ItemStack(WheatItem.ItemId)}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(9, 2);
		}
	}
}