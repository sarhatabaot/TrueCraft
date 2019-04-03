using System;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Logic.Items
{
	public class BowlItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemID = 0x119;

		public override short ID => 0x119;

		public override string DisplayName => "Bowl";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(WoodenPlanksBlock.BlockID), ItemStack.EmptyStack,
					new ItemStack(WoodenPlanksBlock.BlockID)
				},
				{ItemStack.EmptyStack, new ItemStack(WoodenPlanksBlock.BlockID), ItemStack.EmptyStack}
			};

		public ItemStack Output => new ItemStack(ItemID);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(7, 4);
		}
	}
}