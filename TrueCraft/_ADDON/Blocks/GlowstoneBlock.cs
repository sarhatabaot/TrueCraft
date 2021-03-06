using System;
using TrueCraft.Logic;
using TrueCraft._ADDON.Items;

namespace TrueCraft._ADDON.Blocks
{
	public class GlowstoneBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x59;

		public override byte Id => 0x59;

		public override double BlastResistance => 1.5;

		public override double Hardness => 0.3;

		public override byte Luminance => 15;

		public override bool Opaque => false;

		public override string DisplayName => "Glowstone";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Glass;

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(GlowstoneDustItem.ItemId),
					new ItemStack(GlowstoneDustItem.ItemId)
				},
				{
					new ItemStack(GlowstoneDustItem.ItemId),
					new ItemStack(GlowstoneDustItem.ItemId)
				}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(9, 6);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new[]
				{new ItemStack(GlowstoneDustItem.ItemId, (sbyte) new Random().Next(2, 4), descriptor.Metadata)};
		}
	}
}