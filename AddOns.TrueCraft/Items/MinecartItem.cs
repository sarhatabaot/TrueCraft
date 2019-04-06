using System;
using TrueCraft.Logic;

namespace TrueCraft.Items
{
	public class MinecartItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemId = 0x148;

		public override short Id => 0x148;

		public override sbyte MaximumStack => 1;

		public override string DisplayName => "Minecart";

		public virtual ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(IronIngotItem.ItemId), ItemStack.EmptyStack, new ItemStack(IronIngotItem.ItemId)},
				{
					new ItemStack(IronIngotItem.ItemId), new ItemStack(IronIngotItem.ItemId),
					new ItemStack(IronIngotItem.ItemId)
				}
			};

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(7, 8);
		}
	}
}