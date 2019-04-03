using System;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Logic.Items
{
	public class IronIngotItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemID = 0x109;

		public override short ID => 0x109;

		public override string DisplayName => "Iron Ingot";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(IronBlock.BlockID)}
			};

		public ItemStack Output => new ItemStack(ItemID, 9);

		public bool SignificantMetadata => true;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(7, 1);
		}
	}
}