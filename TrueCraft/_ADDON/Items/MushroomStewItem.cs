using System;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Logic.Items
{
	public class MushroomStewItem : FoodItem, ICraftingRecipe
	{
		public static readonly short ItemID = 0x11A;

		public override short Id => 0x11A;

		public override sbyte MaximumStack => 1;

		public override float Restores => 5;

		public override string DisplayName => "Mushroom Stew";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(RedMushroomBlock.BlockId)},
				{new ItemStack(BrownMushroomBlock.BlockId)},
				{new ItemStack(BowlItem.ItemID)}
			};

		public ItemStack Output => new ItemStack(ItemID);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(8, 4);
		}
	}
}