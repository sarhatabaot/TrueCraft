using System;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Logic.Items
{
	public class PaintingItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemId = 0x141;

		public override short Id => 0x141;

		public override string DisplayName => "Painting";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(StickItem.ItemId), new ItemStack(StickItem.ItemId), new ItemStack(StickItem.ItemId)},
				{new ItemStack(StickItem.ItemId), new ItemStack(WoolBlock.BlockId), new ItemStack(StickItem.ItemId)},
				{new ItemStack(StickItem.ItemId), new ItemStack(StickItem.ItemId), new ItemStack(StickItem.ItemId)}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => true;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(10, 1);
		}
	}
}