using System;

namespace TrueCraft.Logic.Items
{
	public class BookItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemID = 0x154;

		public override short Id => 0x154;

		public override sbyte MaximumStack => 64;

		public override string DisplayName => "Book";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(PaperItem.ItemID)},
				{new ItemStack(PaperItem.ItemID)},
				{new ItemStack(PaperItem.ItemID)}
			};

		public ItemStack Output => new ItemStack(ItemID);

		public bool SignificantMetadata => true;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(11, 3);
		}
	}
}