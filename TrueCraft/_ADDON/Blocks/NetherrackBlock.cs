using System;

namespace TrueCraft.Logic.Blocks
{
	public class NetherrackBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x57;

		public override byte Id => 0x57;

		public override double BlastResistance => 2;

		public override double Hardness => 0.4;

		public override byte Luminance => 0;

		public override string DisplayName => "Netherrack";

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(7, 6);
		}
	}
}