using System;
using TrueCraft.Logic;
using TrueCraft.Logic.Items;

namespace TrueCraft.Items
{
	public class CompassItem : ToolItem, ICraftingRecipe
	{
		public static readonly short ItemId = 0x159;

		public override short Id => 0x159;

		public override string DisplayName => "Compass";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{ItemStack.EmptyStack, new ItemStack(IronIngotItem.ItemId), ItemStack.EmptyStack},
				{
					new ItemStack(IronIngotItem.ItemId), new ItemStack(RedstoneItem.ItemId),
					new ItemStack(IronIngotItem.ItemId)
				},
				{ItemStack.EmptyStack, new ItemStack(IronIngotItem.ItemId), ItemStack.EmptyStack}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(6, 3);
		}
	}
}