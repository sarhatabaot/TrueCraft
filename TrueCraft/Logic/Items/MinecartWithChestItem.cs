using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.Core.Logic.Blocks;

namespace TrueCraft.Core.Logic.Items
{
	public class MinecartWithChestItem : MinecartItem, ICraftingRecipe
	{
		public new static readonly short ItemID = 0x156;

		public override short ID => 0x156;

		public override string DisplayName => "Minecart with Chest";

		public override ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(ChestBlock.BlockID)},
				{new ItemStack(MinecartItem.ItemID)}
			};
	}
}