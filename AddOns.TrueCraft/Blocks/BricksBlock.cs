using System;
using TrueCraft.Logic;
using TrueCraft._ADDON.Items;

namespace TrueCraft.Blocks
{
	public class BricksBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x2D;

		public override byte Id => 0x2D;

		public override double BlastResistance => 30;

		public override double Hardness => 2;

		public override byte Luminance => 0;

		public override string DisplayName => "Bricks";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(BrickItem.ItemId), new ItemStack(BrickItem.ItemId)},
				{new ItemStack(BrickItem.ItemId), new ItemStack(BrickItem.ItemId)}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(7, 0);
		}
	}
}