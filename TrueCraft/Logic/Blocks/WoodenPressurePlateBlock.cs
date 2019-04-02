using TrueCraft.API;
using TrueCraft.API.Logic;

namespace TrueCraft.Core.Logic.Blocks
{
	public class WoodenPressurePlateBlock : PressurePlateBlock, ICraftingRecipe
	{
		public static readonly byte BlockID = 0x48;

		public override byte ID => 0x48;

		public override string DisplayName => "Wooden Pressure Plate";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(WoodenPlanksBlock.BlockID), new ItemStack(WoodenPlanksBlock.BlockID)}
			};

		public ItemStack Output => new ItemStack(BlockID);

		public bool SignificantMetadata => false;
	}
}