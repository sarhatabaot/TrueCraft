using System;
using TrueCraft.Logic;

namespace TrueCraft.Blocks
{
	public class DoubleSlabBlock : SlabBlock
	{
		public new static readonly byte BlockId = 0x2B;

		public override byte Id => 0x2B;

		public override double BlastResistance => 30;

		public override double Hardness => 2;

		public override byte Luminance => 0;

		public override string DisplayName => "Double Stone Slab";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Wood;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(6, 0);
		}
	}
}