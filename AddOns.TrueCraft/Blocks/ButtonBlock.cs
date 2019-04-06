using TrueCraft.Logic;

namespace TrueCraft.Blocks
{
	public class ButtonBlock : BlockProvider, ICraftingRecipe
	{
		public static readonly byte BlockId = 0x4D;

		public override byte Id => 0x4D;

		public override double BlastResistance => 2.5;

		public override double Hardness => 0.5;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Button";

		public ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(StoneBlock.BlockId)},
				{new ItemStack(StoneBlock.BlockId)}
			};

		public ItemStack Output => new ItemStack(BlockId);

		public bool SignificantMetadata => false;
	}
}