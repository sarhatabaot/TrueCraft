using TrueCraft.Logic;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Items
{
	public class MinecartWithChestItem : MinecartItem, ICraftingRecipe
	{
		public new static readonly short ItemId = 0x156;

		public override short Id => 0x156;

		public override string DisplayName => "Minecart with Chest";

		public override ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(ChestBlock.BlockId)},
				{new ItemStack(MinecartItem.ItemId)}
			};
	}
}