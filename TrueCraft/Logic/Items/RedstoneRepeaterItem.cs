using System;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Logic.Items
{
	public class RedstoneRepeaterItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemID = 0x164;

		public override short ID => 0x164;

		public override string DisplayName => "Redstone Repeater";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(RedstoneTorchBlock.BlockID), new ItemStack(RedstoneDustBlock.BlockID),
					new ItemStack(RedstoneTorchBlock.BlockID)
				},
				{
					new ItemStack(StoneBlock.BlockID), new ItemStack(StoneBlock.BlockID),
					new ItemStack(StoneBlock.BlockID)
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