using System;
using TrueCraft.Logic;

namespace TrueCraft.Blocks
{
	public class SpongeBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x13;

		public override byte Id => 0x13;

		public override double BlastResistance => 3;

		public override double Hardness => 0.6;

		public override byte Luminance => 0;

		public override string DisplayName => "Sponge";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Grass;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(0, 3);
		}
	}
}