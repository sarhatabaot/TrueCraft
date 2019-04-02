using System;
using TrueCraft.API;
using TrueCraft.API.Logic;
using TrueCraft.Core.Logic.Items;

namespace TrueCraft.Core.Logic.Blocks
{
	public class RailBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockID = 0x42;

		public override byte ID => 0x42;

		public override double BlastResistance => 3.5;

		public override double Hardness => 0.7;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Rail";

		public virtual ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(IronIngotItem.ItemID),
					ItemStack.EmptyStack,
					new ItemStack(IronIngotItem.ItemID)
				},
				{
					new ItemStack(IronIngotItem.ItemID),
					new ItemStack(StickItem.ItemID),
					new ItemStack(IronIngotItem.ItemID)
				},
				{
					new ItemStack(IronIngotItem.ItemID),
					ItemStack.EmptyStack,
					new ItemStack(IronIngotItem.ItemID)
				}
			};

		public ItemStack Output => new ItemStack(BlockID);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(0, 8);
		}
	}
}