using System;
using TrueCraft.Logic;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft._ADDON.Items
{
	public class BowlItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemId = 0x119;

		public override short Id => 0x119;

		public override string DisplayName => "Bowl";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(WoodenPlanksBlock.BlockId), ItemStack.EmptyStack,
					new ItemStack(WoodenPlanksBlock.BlockId)
				},
				{ItemStack.EmptyStack, new ItemStack(WoodenPlanksBlock.BlockId), ItemStack.EmptyStack}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(7, 4);
		}
	}
}