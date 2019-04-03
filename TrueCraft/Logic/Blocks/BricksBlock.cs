using System;
using TrueCraft.Logic.Items;

namespace TrueCraft.Logic.Blocks
{
	public class BricksBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockID = 0x2D;

		public override byte ID => 0x2D;

		public override double BlastResistance => 30;

		public override double Hardness => 2;

		public override byte Luminance => 0;

		public override string DisplayName => "Bricks";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(BrickItem.ItemID), new ItemStack(BrickItem.ItemID)},
				{new ItemStack(BrickItem.ItemID), new ItemStack(BrickItem.ItemID)}
			};

		public ItemStack Output => new ItemStack(BlockID);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(7, 0);
		}
	}
}