using System;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.Logic.Blocks
{
	public class LeavesBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x12;

		public override byte Id => 0x12;

		public override double BlastResistance => 1;

		public override double Hardness => 0.2;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override bool DiffuseSkyLight => true;

		public override byte LightOpacity => 2;

		public override string DisplayName => "Leaves";

		public override bool Flammable => true;

		public override SoundEffectClass SoundEffect => SoundEffectClass.Grass;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(4, 3);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			var provider = ItemRepository.GetItemProvider(item.Id);
			if (provider is IShearLeaves)
				return base.GetDrop(descriptor, item);

			if (MathHelper.Random.Next(20) == 0) // 5% chance
				return new[] {new ItemStack(SaplingBlock.BlockId, 1, descriptor.Metadata)};
			return new ItemStack[0];
		}
	}
}