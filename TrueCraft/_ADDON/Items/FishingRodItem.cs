using System;

namespace TrueCraft.Logic.Items
{
	public class FishingRodItem : ToolItem, ICraftingRecipe
	{
		public static readonly short ItemId = 0x15A;

		public override short Id => 0x15A;

		public override sbyte MaximumStack => 1;

		public override short BaseDurability => 65;

		public override string DisplayName => "Fishing Rod";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{ItemStack.EmptyStack, ItemStack.EmptyStack, new ItemStack(StickItem.ItemId)},
				{ItemStack.EmptyStack, new ItemStack(StickItem.ItemId), new ItemStack(StringItem.ItemId)},
				{new ItemStack(StickItem.ItemId), ItemStack.EmptyStack, new ItemStack(StringItem.ItemId)}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(5, 4);
		}
	}
}