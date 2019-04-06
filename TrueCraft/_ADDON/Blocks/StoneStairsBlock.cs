namespace TrueCraft.Logic.Blocks
{
	public class StoneStairsBlock : StairsBlock, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x43;

		public override byte Id => 0x43;

		public override double BlastResistance => 30;

		public override string DisplayName => "Stone Stairs";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(StoneBlock.BlockId), ItemStack.EmptyStack, ItemStack.EmptyStack},
				{new ItemStack(StoneBlock.BlockId), new ItemStack(StoneBlock.BlockId), ItemStack.EmptyStack},
				{
					new ItemStack(StoneBlock.BlockId), new ItemStack(StoneBlock.BlockId),
					new ItemStack(StoneBlock.BlockId)
				}
			};

		public ItemStack Output => new ItemStack(BlockId);
	}
}