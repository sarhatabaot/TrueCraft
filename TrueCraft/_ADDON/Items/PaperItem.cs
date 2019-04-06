using System;

namespace TrueCraft.Logic.Items
{
	public class PaperItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemId = 0x153;

		public override short Id => 0x153;

		public override string DisplayName => "Paper";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(SugarCanesItem.ItemId), new ItemStack(SugarCanesItem.ItemId),
					new ItemStack(SugarCanesItem.ItemId)
				}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => true;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(10, 3);
		}
	}
}