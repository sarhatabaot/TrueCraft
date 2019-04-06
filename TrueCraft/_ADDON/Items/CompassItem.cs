using System;

namespace TrueCraft.Logic.Items
{
	public class CompassItem : ToolItem, ICraftingRecipe
	{
		public static readonly short ItemID = 0x159;

		public override short Id => 0x159;

		public override string DisplayName => "Compass";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{ItemStack.EmptyStack, new ItemStack(IronIngotItem.ItemID), ItemStack.EmptyStack},
				{
					new ItemStack(IronIngotItem.ItemID), new ItemStack(RedstoneItem.ItemID),
					new ItemStack(IronIngotItem.ItemID)
				},
				{ItemStack.EmptyStack, new ItemStack(IronIngotItem.ItemID), ItemStack.EmptyStack}
			};

		public ItemStack Output => new ItemStack(ItemID);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(6, 3);
		}
	}
}