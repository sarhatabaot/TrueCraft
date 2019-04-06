using System;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Logic.Items
{
	public class RedstoneRepeaterItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemID = 0x164;

		public override short Id => 0x164;

		public override string DisplayName => "Redstone Repeater";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(RedstoneTorchBlock.BlockId), new ItemStack(RedstoneDustBlock.BlockId),
					new ItemStack(RedstoneTorchBlock.BlockId)
				},
				{
					new ItemStack(StoneBlock.BlockId), new ItemStack(StoneBlock.BlockId),
					new ItemStack(StoneBlock.BlockId)
				}
			};

		public ItemStack Output => new ItemStack(ItemID);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(6, 5);
		}
	}
}