using TrueCraft.Logic;
using TrueCraft.Logic.Blocks;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.Blocks
{
	public class StonePressurePlateBlock : PressurePlateBlock, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x46;

		public override byte Id => 0x46;

		public override string DisplayName => "Stone Pressure Plate";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(StoneBlock.BlockId), new ItemStack(StoneBlock.BlockId)}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;
	}
}