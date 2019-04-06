using System;
using TrueCraft.Logic.Items;

namespace TrueCraft.Logic.Blocks
{
	public class WoolBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x23;

		public override byte Id => 0x23;

		public override double BlastResistance => 4;

		public override double Hardness => 0.8;

		public override byte Luminance => 0;

		public override string DisplayName => "Wool";

		public override bool Flammable => true;

		public override SoundEffectClass SoundEffect => SoundEffectClass.Cloth;

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(StringItem.ItemId), new ItemStack(StringItem.ItemId)},
				{new ItemStack(StringItem.ItemId), new ItemStack(StringItem.ItemId)}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => true;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(0, 4);
		}
	}
}