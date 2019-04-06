using System;
using TrueCraft.Logic;
using TrueCraft._ADDON.Items;

namespace TrueCraft._ADDON.Blocks
{
	public class LeverBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x45;

		public override byte Id => 0x45;

		public override double BlastResistance => 2.5;

		public override double Hardness => 0.5;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Lever";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(StickItem.ItemId)},
				{new ItemStack(CobblestoneBlock.BlockId)}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(0, 6);
		}
	}
}