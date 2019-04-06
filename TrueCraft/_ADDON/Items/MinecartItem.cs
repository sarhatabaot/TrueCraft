using System;

namespace TrueCraft.Logic.Items
{
	public class MinecartItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemID = 0x148;

		public override short Id => 0x148;

		public override sbyte MaximumStack => 1;

		public override string DisplayName => "Minecart";

		public virtual ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(IronIngotItem.ItemID), ItemStack.EmptyStack, new ItemStack(IronIngotItem.ItemID)},
				{
					new ItemStack(IronIngotItem.ItemID), new ItemStack(IronIngotItem.ItemID),
					new ItemStack(IronIngotItem.ItemID)
				}
			};

		public ItemStack Output => new ItemStack(ItemID);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(7, 8);
		}
	}
}