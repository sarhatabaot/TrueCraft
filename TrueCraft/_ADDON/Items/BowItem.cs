using System;
using TrueCraft.Logic;

namespace TrueCraft._ADDON.Items
{
	public class BowItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemId = 0x105;

		public override short Id => 0x105;

		public override sbyte MaximumStack => 1;

		public override string DisplayName => "Bow";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{ItemStack.EmptyStack, new ItemStack(StickItem.ItemId), new ItemStack(StringItem.ItemId)},
				{new ItemStack(StickItem.ItemId), ItemStack.EmptyStack, new ItemStack(StringItem.ItemId)},
				{ItemStack.EmptyStack, new ItemStack(StickItem.ItemId), new ItemStack(StringItem.ItemId)}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(5, 1);
		}
	}
}