using System;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.Logic.Blocks
{
	public class StoneBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x01;

		public override byte Id => 0x01;

		public override double BlastResistance => 30;

		public override double Hardness => 1.5;

		public override byte Luminance => 0;

		public override string DisplayName => "Stone";

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(1, 0);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			var provider = ItemRepository.GetItemProvider(item.Id);
			if (provider is IBreakStone)
				return new[] {new ItemStack(CobblestoneBlock.BlockId, 1, descriptor.Metadata)};
			return new ItemStack[0];
		}
	}
}