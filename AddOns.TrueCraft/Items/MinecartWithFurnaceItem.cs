using TrueCraft.Logic;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Items
{
	public class MinecartWithFurnaceItem : MinecartItem, ICraftingRecipe
	{
		public new static readonly short ItemId = 0x157;

		public override short Id => 0x157;

		public override string DisplayName => "Minecart with Furnace";

		public override ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(FurnaceBlock.BlockId)},
				{new ItemStack(MinecartItem.ItemId)}
			};
	}
}