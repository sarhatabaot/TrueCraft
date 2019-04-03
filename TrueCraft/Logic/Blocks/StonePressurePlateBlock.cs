namespace TrueCraft.Logic.Blocks
{
	public class StonePressurePlateBlock : PressurePlateBlock, ICraftingRecipe
	{
		public static readonly byte BlockID = 0x46;

		public override byte ID => 0x46;

		public override string DisplayName => "Stone Pressure Plate";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(StoneBlock.BlockID), new ItemStack(StoneBlock.BlockID)}
			};

		public ItemStack Output => new ItemStack(BlockID);

		public bool SignificantMetadata => false;
	}
}