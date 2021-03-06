using System;
using TrueCraft.Logic;
using TrueCraft._ADDON.Items;

namespace TrueCraft._ADDON.Blocks
{
	public class ClayBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x52;

		public override byte Id => 0x52;

		public override double BlastResistance => 3;

		public override double Hardness => 0.6;

		public override byte Luminance => 0;

		public override string DisplayName => "Clay";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Gravel;

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(ClayItem.ItemId), new ItemStack(ClayItem.ItemId)},
				{new ItemStack(ClayItem.ItemId), new ItemStack(ClayItem.ItemId)}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(8, 4);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new[] {new ItemStack(ClayItem.ItemId, 4)};
		}
	}
}