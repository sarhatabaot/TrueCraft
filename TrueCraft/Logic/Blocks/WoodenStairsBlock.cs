using System;
using TrueCraft.API;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
	public class WoodenStairsBlock : StairsBlock, ICraftingRecipe, IBurnableItem
	{
		public static readonly byte BlockID = 0x35;

		public override byte ID => 0x35;

		public override double BlastResistance => 15;

		public override string DisplayName => "Wooden Stairs";

		public override bool Flammable => true;

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(WoodenPlanksBlock.BlockID), ItemStack.EmptyStack, ItemStack.EmptyStack},
				{
					new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID),
					ItemStack.EmptyStack
				},
				{
					new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID),
					new ItemStack(WoodenPlanksBlock.BlockID)
				}
			};

		public ItemStack Output => new ItemStack(BlockID);
	}
}