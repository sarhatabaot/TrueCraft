using System;

namespace TrueCraft.Logic.Blocks
{
	public class WoodenPlanksBlock : BlockProvider, ICraftingRecipe, IBurnableItem
	{
		public static readonly byte BlockId = 0x05;

		public override byte Id => 0x05;

		public override double BlastResistance => 15;

		public override double Hardness => 2;

		public override byte Luminance => 0;

		public override string DisplayName => "Wooden Planks";

		public override bool Flammable => true;

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public TimeSpan BurnTime => TimeSpan.FromSeconds(15);

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(WoodBlock.BlockId)}
			};

		public ItemStack Output => new ItemStack(BlockId, 4);

		public bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(4, 0);
		}
	}
}