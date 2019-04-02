using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
	public class MinecartWithFurnaceItem : MinecartItem, ICraftingRecipe
	{
		public new static readonly short ItemID = 0x157;

		public override short ID => 0x157;

		public override string DisplayName => "Minecart with Furnace";

		public override ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(FurnaceBlock.BlockID)},
				{new ItemStack(MinecartItem.ItemID)}
			};
	}
}