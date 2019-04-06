using System;
using TrueCraft.Blocks;
using TrueCraft.Logic;
using TrueCraft.Logic.Blocks;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.Items
{
	public class RedstoneRepeaterItem : ItemProvider, ICraftingRecipe
	{
		public static readonly short ItemId = 0x164;

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

		public ItemStack Output => new ItemStack(ItemId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetIconTexture(byte metadata)
		{
			return new Tuple<int, int>(6, 5);
		}
	}
}