using System;

namespace TrueCraft.Logic.Blocks
{
	public class PortalBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x5A;

		public override byte Id => 0x5A;

		public override double BlastResistance => 0;

		public override double Hardness => -1;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override string DisplayName => "Portal";

		public override SoundEffectClass SoundEffect => SoundEffectClass.Glass;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(14, 0);
		}
	}
}