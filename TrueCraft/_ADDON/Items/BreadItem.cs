using System;

namespace TrueCraft.Logic.Items
{
	public class BreadItem : FoodItem, ICraftingRecipe
	{
		public static readonly short ItemID = 0x129;

		public override short Id => 0x129;

		public override float Restores => 2.5f;

		public override string DisplayName => "Bread";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(WheatItem.ItemID), new ItemStack(WheatItem.ItemID), new ItemStack(WheatItem.ItemID)}
			};

		public ItemStack Output => new ItemStack(ItemID);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(9, 2);
		}
	}
}