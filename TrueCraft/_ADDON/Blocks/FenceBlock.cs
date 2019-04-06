using System;
using TrueCraft.Logic.Items;

namespace TrueCraft.Logic.Blocks
{
	public class FenceBlock : BlockProvider, ICraftingRecipe, IBurnableItem
	{
		public static readonly byte BlockId = 0x55;

		public override byte Id => 0x55;

		public override double BlastResistance => 15;

		public override double Hardness => 2;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override bool Flammable => true;

		public override string DisplayName => "Fence";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(StickItem.ItemId),
					new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(StickItem.ItemId)
				},
				{
					new ItemStack(StickItem.ItemId),
					new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(StickItem.ItemId)
				}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(4, 0);
		}
	}
}