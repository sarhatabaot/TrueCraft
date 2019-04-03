using System;
using TrueCraft.Logic.Items;

namespace TrueCraft.Logic.Blocks
{
	public class SnowBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockID = 0x50;

		public override byte ID => 0x50;

		public override double BlastResistance => 1;

		public override double Hardness => 0.2;

		public override byte Luminance => 0;

		public override string DisplayName => "Snow Block";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Snow;

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(SnowballItem.ItemID), new ItemStack(SnowballItem.ItemID)},
				{new ItemStack(SnowballItem.ItemID), new ItemStack(SnowballItem.ItemID)}
			};

		public ItemStack Output => new ItemStack(BlockID);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(2, 4);
		}
	}
}