using System;
using TrueCraft.Logic;

namespace TrueCraft._ADDON.Blocks
{
	public class MonsterSpawnerBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x34;

		public override byte Id => 0x34;

		public override double BlastResistance => 25;

		public override double Hardness => 5;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Monster Spawner";

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(1, 4);
		}

		protected override ItemStack[] GetDrop(BlockDescriptor descriptor, ItemStack item)
		{
			return new ItemStack[0];
		}
	}
}