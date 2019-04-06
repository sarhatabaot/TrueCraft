using System;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Logic.Items
{
	public class BoatItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemID = 0x14D;

		public override short Id => 0x14D;

		public override sbyte MaximumStack => 1;

		public override string DisplayName => "Boat";

		public virtual ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(WoodenPlanksBlock.BlockId),
					ItemStack.EmptyStack,
					new ItemStack(WoodenPlanksBlock.BlockId)
				},
				{
					new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId)
				}
			};

		public ItemStack Output => new ItemStack(ItemID);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(8, 8);
		}
	}
}