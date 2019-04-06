using System;
using TrueCraft.Logic;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.Blocks
{
	public class WoodenStairsBlock : StairsBlock, ICraftingRecipe, IBurnableItem
	{
		public static readonly byte BlockId = 0x35;

		public override byte Id => 0x35;

		public override double BlastResistance => 15;

		public override string DisplayName => "Wooden Stairs";

		public override bool Flammable => true;

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(WoodenPlanksBlock.BlockId), ItemStack.EmptyStack, ItemStack.EmptyStack},
				{
					new ItemStack(WoodenPlanksBlock.BlockId), new ItemStack(WoodenPlanksBlock.BlockId),
					ItemStack.EmptyStack
				},
				{
					new ItemStack(WoodenPlanksBlock.BlockId), new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId)
				}
			};

		public ItemStack Output => new ItemStack(BlockId);
	}
}