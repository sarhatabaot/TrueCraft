using System;

namespace TrueCraft.Logic.Blocks
{
	public class WaterBlock : FluidBlock
	{
		public static readonly byte BlockId = 0x08;

		public override byte Id => 0x08;

		public override double BlastResistance => 500;

		public override double Hardness => 100;

		public override byte Luminance => 0;

		public override bool Opaque => false;

		public override byte LightOpacity => 2;

		public override string DisplayName => "Water";

		protected override double SecondsBetweenUpdates => 0.25;

		protected override byte MaximumFluidDepletion => 7;

		protected override byte FlowingID => BlockId;

		protected override byte StillID => StationaryWaterBlock.BlockId;

		public override Tuple<int, int> GetTextureMap(byte metadata)
		{
			return new Tuple<int, int>(13, 12);
		}
	}
}