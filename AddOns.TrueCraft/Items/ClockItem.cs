using System;
using TrueCraft.Logic;
using TrueCraft._ADDON.Items;

namespace TrueCraft.Items
{
	public class ClockItem : ToolItem, ICraftingRecipe
	{
		public static readonly short ItemId = 0x15B;

		public override short Id => 0x15B;

		public override string DisplayName => "Clock";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{ItemStack.EmptyStack, new ItemStack(GoldIngotItem.ItemId), ItemStack.EmptyStack},
				{
					new ItemStack(GoldIngotItem.ItemId), new ItemStack(RedstoneItem.ItemId),
					new ItemStack(GoldIngotItem.ItemId)
				},
				{ItemStack.EmptyStack, new ItemStack(GoldIngotItem.ItemId), ItemStack.EmptyStack}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(6, 4);
		}
	}
}