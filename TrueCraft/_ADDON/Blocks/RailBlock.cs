using System;
using TrueCraft.Logic.Items;

namespace TrueCraft.Logic.Blocks
{
	public class RailBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x42;

		public override byte Id => 0x42;

		public override double BlastResistance => 3.5;

		public override double Hardness => 0.7;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Rail";

		public virtual ItemStack[,] Pattern =>
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
					ItemStack.EmptyStack,
					new ItemStack(IronIngotItem.ItemId)
				}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(0, 8);
		}
	}
}