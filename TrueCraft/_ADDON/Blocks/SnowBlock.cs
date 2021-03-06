using System;
using TrueCraft.Logic;
using TrueCraft._ADDON.Items;

namespace TrueCraft._ADDON.Blocks
{
	public class SnowBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x50;

		public override byte Id => 0x50;

		public override double BlastResistance => 1;

		public override double Hardness => 0.2;

		public override byte Luminance => 0;

		public override string DisplayName => "Snow Block";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Snow;

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(SnowballItem.ItemId), new ItemStack(SnowballItem.ItemId)},
				{new ItemStack(SnowballItem.ItemId), new ItemStack(SnowballItem.ItemId)}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(2, 4);
		}
	}
}