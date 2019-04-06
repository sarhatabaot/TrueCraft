using System;
using TrueCraft.Items;
using TrueCraft.Logic.Blocks;
using TrueCraft.Logic.Items;

namespace TrueCraft.Blocks
{
	public class PoweredRailBlock : RailBlock
	{
		public new static readonly byte BlockId = 0x1B;

		public override byte Id => 0x1B;

		public override string DisplayName => "Powered Rail";

		public override ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(IronIngotItem.ItemId),
					ItemStack.EmptyStack,
					new ItemStack(IronIngotItem.ItemId)
				},
				{
					new ItemStack(IronIngotItem.ItemId),
					new ItemStack(StickItem.ItemId),
					new ItemStack(IronIngotItem.ItemId)
				},
				{
					new ItemStack(IronIngotItem.ItemId),
					new ItemStack(RedstoneDustBlock.BlockId),
					new ItemStack(IronIngotItem.ItemId)
				}
			};

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(3, 11);
		}
	}
}