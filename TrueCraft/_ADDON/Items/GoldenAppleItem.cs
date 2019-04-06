using System;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Logic.Items
{
	public class GoldenAppleItem : FoodItem, ICraftingRecipe
	{
		public static readonly short ItemID = 0x142;

		public override short Id => 0x142;

		public override float Restores => 10;

		public override string DisplayName => "Golden Apple";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(GoldBlock.BlockId),
					new ItemStack(GoldBlock.BlockId),
					new ItemStack(GoldBlock.BlockId)
				},
				{
					new ItemStack(GoldBlock.BlockId),
					new ItemStack(AppleItem.ItemId),
					new ItemStack(GoldBlock.BlockId)
				},
				{
					new ItemStack(GoldBlock.BlockId),
					new ItemStack(GoldBlock.BlockId),
					new ItemStack(GoldBlock.BlockId)
				}
			};

		public ItemStack Output => new ItemStack(ItemID);

		public bool SignificantMetadata => true;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(11, 0);
		}
	}
}