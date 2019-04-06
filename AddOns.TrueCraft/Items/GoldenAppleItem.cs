using System;
using TrueCraft.Blocks;
using TrueCraft.Logic;
using TrueCraft.Logic.Blocks;
using TrueCraft.Logic.Items;

namespace TrueCraft.Items
{
	public class GoldenAppleItem : FoodItem, ICraftingRecipe
	{
		public static readonly short ItemId = 0x142;

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

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => true;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(11, 0);
		}
	}
}