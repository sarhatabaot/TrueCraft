using System;
using TrueCraft.Logic;
using TrueCraft._ADDON.Items;

namespace TrueCraft._ADDON.Blocks
{
	public class CobwebBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x1E;

		public override byte Id => 0x1E;

		public override double BlastResistance => 20;

		public override double Hardness => 4;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override bool DiffuseSkyLight => true;

		public override ToolType EffectiveTools => ToolType.Sword;

		public override string DisplayName => "Cobweb";

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(11, 0);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new[] {new ItemStack(StringItem.ItemId, 1, descriptor.Metadata)};
		}
	}
}