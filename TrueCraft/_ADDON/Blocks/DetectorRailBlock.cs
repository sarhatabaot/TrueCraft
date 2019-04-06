using System;
using TrueCraft.Logic.Items;

namespace TrueCraft.Logic.Blocks
{
	public class DetectorRailBlock : RailBlock
	{
		public new static readonly byte BlockId = 0x1C;

		public override byte Id => 0x1C;

		public override string DisplayName => "Detector Rail";

		public override ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(IronIngotItem.ItemID),
					ItemStack.EmptyStack,
					new ItemStack(IronIngotItem.ItemID)
				},
				{
					new ItemStack(IronIngotItem.ItemID),
					new ItemStack(StonePressurePlateBlock.BlockId),
					new ItemStack(IronIngotItem.ItemID)
				},
				{
					new ItemStack(IronIngotItem.ItemID),
					new ItemStack(RedstoneDustBlock.BlockId),
					new ItemStack(IronIngotItem.ItemID)
				}
			};

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(3, 12);
		}
	}
}