using System;
using TrueCraft.Logic;
using TrueCraft._ADDON.Items;

namespace TrueCraft._ADDON.Blocks
{
	public class RedstoneDustBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x37;

		public override byte Id => 0x37;

		public override double BlastResistance => 0;

		public override double Hardness => 0;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Redstone Dust";

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(4, 10);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new[] {new ItemStack(RedstoneItem.ItemId, 1, descriptor.Metadata)};
		}

		public override Coordinates3D GetSupportDirection(BlockDescriptor descriptor)
		{
			return Coordinates3D.Down;
		}
	}
}