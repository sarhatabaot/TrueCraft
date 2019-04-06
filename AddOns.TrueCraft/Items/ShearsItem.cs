using System;
using TrueCraft.Logic;
using TrueCraft.Logic.Items;

namespace TrueCraft.Items
{
	public class ShearsItem : ToolItem, ICraftingRecipe
	{
		public static readonly short ItemId = 0x167;

		public override short Id => 0x167;

		public override sbyte MaximumStack => 1;

		public override short BaseDurability => 239;

		public override string DisplayName => "Shears";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{ItemStack.EmptyStack, new ItemStack(IronIngotItem.ItemId)},
				{new ItemStack(IronIngotItem.ItemId), ItemStack.EmptyStack}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(13, 5);
		}
	}
}