using System;

namespace TrueCraft.Logic.Blocks
{
	public class BedrockBlock : BlockProvider
	{
		public static readonly byte BlockId = 0x07;

		public override byte Id => 0x07;

		public override double BlastResistance => 18000000;

		public override double Hardness => -1;

		public override byte Luminance => 0;

		public override string DisplayName => "Bedrock";

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(1, 1);
		}
	}
}