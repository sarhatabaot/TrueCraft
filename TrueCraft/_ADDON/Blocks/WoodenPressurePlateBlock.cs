using TrueCraft.Logic;

namespace TrueCraft._ADDON.Blocks
{
	public class WoodenPressurePlateBlock : PressurePlateBlock, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x48;

		public override byte Id => 0x48;

		public override string DisplayName => "Wooden Pressure Plate";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(WoodenPlanksBlock.BlockId), new ItemStack(WoodenPlanksBlock.BlockId)}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;
	}
}