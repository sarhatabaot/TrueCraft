using System;
using TrueCraft.Logic.Items;

namespace TrueCraft.Logic.Blocks
{
	public class RedstoneTorchBlock : TorchBlock, ICraftingRecipe
	{
		public new static readonly byte BlockId = 0x4C;

		public override byte Id => 0x4C;

		public override double BlastResistance => 0;

		public override double Hardness => 0;

		public override byte Luminance => 7;

		public override bool Opaque => false;

		public override string DisplayName => "Redstone Torch";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public override ItemStack[,] Pattern =>
			new[,]
			{
				{new ItemStack(RedstoneDustBlock.BlockId)},
				{new ItemStack(StickItem.ItemId)}
			};

		public override ItemStack Output => new ItemStack(BlockId);

		public override bool SignificantMetadata => false;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(3, 6);
		}
	}
}