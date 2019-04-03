using System;
using TrueCraft.Logic.Items;

namespace TrueCraft.Logic.Blocks
{
	public class RedstoneOreBlock : BlockProvider, ISmeltableItem
	{
		public static readonly byte BlockID = 0x49;

		public override byte ID => 0x49;

		public override double BlastResistance => 15;

		public override double Hardness => 3;

		public override byte Luminance => 0;

		public override string DisplayName => "Redstone Ore";

		public ItemStack SmeltingOutput => new ItemStack(RedstoneItem.ItemID);

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(3, 3);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new[] {new ItemStack(RedstoneItem.ItemID, (sbyte) new Random().Next(4, 5), descriptor.Metadata)};
		}
	}
}