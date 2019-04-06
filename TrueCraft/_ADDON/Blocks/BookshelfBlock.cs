using System;
using TrueCraft.Logic.Items;

namespace TrueCraft.Logic.Blocks
{
	public class BookshelfBlock : BlockProvider, ICraftingRecipe, IBurnableItem
	{
		public static readonly byte BlockId = 0x2F;

		public override byte Id => 0x2F;

		public override double BlastResistance => 7.5;

		public override double Hardness => 1.5;

		public override byte Luminance => 0;

		public override string DisplayName => "Bookshelf";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public override bool Flammable => true;

		public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

		public ItemStack[,] Pattern =>
			new[,]
			{
				{
					new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId)
				},
				{
					new ItemStack(BookItem.ItemId),
					new ItemStack(BookItem.ItemId),
					new ItemStack(BookItem.ItemId)
				},
				{
					new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId),
					new ItemStack(WoodenPlanksBlock.BlockId)
				}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(3, 2);
		}
	}
}