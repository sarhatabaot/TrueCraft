using System;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Logic.Items
{
	public class BoatItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemID = 0x14D;

		public override short ID => 0x14D;

		public override sbyte MaximumStack => 1;

		public override string DisplayName => "Boat";

		public virtual ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(WoodenPlanksBlock.BlockID),
					ItemStack.EmptyStack,
					new ItemStack(WoodenPlanksBlock.BlockID)
				},
				{
					new ItemStack(WoodenPlanksBlock.BlockID),
					new ItemStack(WoodenPlanksBlock.BlockID),
					new ItemStack(WoodenPlanksBlock.BlockID)
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