using System;
using TrueCraft.Logic;
using TrueCraft._ADDON.Items;

namespace TrueCraft._ADDON.Blocks
{
	public class RedstoneOreBlock : BlockProvider, ISmeltableItem
	{
		public static readonly byte BlockId = 0x49;

		public override byte Id => 0x49;

		public override double BlastResistance => 15;

		public override double Hardness => 3;

		public override byte Luminance => 0;

		public override string DisplayName => "Redstone Ore";

		public ItemStack SmeltingOutput => new ItemStack(RedstoneItem.ItemId);

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(3, 3);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new[] {new ItemStack(RedstoneItem.ItemId, (sbyte) new Random().Next(4, 5), descriptor.Metadata)};
		}
	}
}