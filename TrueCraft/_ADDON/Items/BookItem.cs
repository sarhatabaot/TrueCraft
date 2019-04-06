using System;
using TrueCraft.Logic;

namespace TrueCraft._ADDON.Items
{
	public class BookItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemId = 0x154;

		public override short Id => 0x154;

		public override sbyte MaximumStack => 64;

		public override string DisplayName => "Book";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(PaperItem.ItemId)},
				{new ItemStack(PaperItem.ItemId)},
				{new ItemStack(PaperItem.ItemId)}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => true;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(11, 3);
		}
	}
}