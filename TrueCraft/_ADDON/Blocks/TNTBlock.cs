using System;
using TrueCraft.Logic;
using TrueCraft._ADDON.Items;

namespace TrueCraft._ADDON.Blocks
{
	public class TNTBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x2E;

		public override byte Id => 0x2E;

		public override double BlastResistance => 0;

		public override double Hardness => 0;

		public override byte Luminance => 0;

		public override string DisplayName => "TNT";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Grass;

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(GunpowderItem.ItemId),
					new ItemStack(SandBlock.BlockId),
					new ItemStack(GunpowderItem.ItemId)
				},
				{
					new ItemStack(SandBlock.BlockId),
					new ItemStack(GunpowderItem.ItemId),
					new ItemStack(SandBlock.BlockId)
				},
				{
					new ItemStack(GunpowderItem.ItemId),
					new ItemStack(SandBlock.BlockId),
					new ItemStack(GunpowderItem.ItemId)
				}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(8, 0);
		}
	}
}