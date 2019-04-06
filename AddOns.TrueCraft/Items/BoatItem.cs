using System;
using TrueCraft.Logic;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.Items
{
	public class BoatItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemId = 0x14D;

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

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(8, 8);
		}
	}
}