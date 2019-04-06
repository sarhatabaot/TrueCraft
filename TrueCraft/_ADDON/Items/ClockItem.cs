using System;

namespace TrueCraft.Logic.Items
{
	public class ClockItem : ToolItem, ICraftingRecipe
	{
		public static readonly short ItemID = 0x15B;

		public override short Id => 0x15B;

		public override string DisplayName => "Clock";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{ItemStack.EmptyStack, new ItemStack(GoldIngotItem.ItemID), ItemStack.EmptyStack},
				{
					new ItemStack(GoldIngotItem.ItemID), new ItemStack(RedstoneItem.ItemID),
					new ItemStack(GoldIngotItem.ItemID)
				},
				{ItemStack.EmptyStack, new ItemStack(GoldIngotItem.ItemID), ItemStack.EmptyStack}
			};

		public ItemStack Output => new ItemStack(ItemID);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(6, 4);
		}
	}
}