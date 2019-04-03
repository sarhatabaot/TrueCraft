namespace TrueCraft.Logic.Blocks
{
	public class StoneStairsBlock : StairsBlock, ICraftingRecipe
	{
		public static readonly byte BlockID = 0x43;

		public override byte ID => 0x43;

		public override double BlastResistance => 30;

		public override string DisplayName => "Stone Stairs";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(StoneBlock.BlockID), ItemStack.EmptyStack, ItemStack.EmptyStack},
				{new ItemStack(StoneBlock.BlockID), new ItemStack(StoneBlock.BlockID), ItemStack.EmptyStack},
				{
					new ItemStack(StoneBlock.BlockID), new ItemStack(StoneBlock.BlockID),
					new ItemStack(StoneBlock.BlockID)
				}
			};

		public ItemStack Output => new ItemStack(BlockID);
	}
}