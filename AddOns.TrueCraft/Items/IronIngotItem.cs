using System;
using TrueCraft.Blocks;
using TrueCraft.Logic;

namespace TrueCraft.Items
{
	public class IronIngotItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemId = 0x109;

		public override short Id => 0x109;

		public override string DisplayName => "Iron Ingot";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(IronBlock.BlockId)}
			};

		public ItemStack Output => new ItemStack(ItemId, 9);

		public bool SignificantMetadata => true;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(7, 1);
		}
	}
}