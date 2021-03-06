namespace TrueCraft._ADDON.Blocks
{
	public class LavaBlock : FluidBlock
	{
		public static readonly byte BlockId = 0x0A;

		public LavaBlock() : this(false)
		{
		}

		public LavaBlock(bool nether)
		{
			if (nether)
				_MaximumFluidDepletion = 7;
			else
				_MaximumFluidDepletion = 3;
		}

		public override byte Id => 0x0A;

		public override double BlastResistance => 0;

		public override double Hardness => 100;

		public override byte Luminance => 15;

		public override bool Opaque => false;

		public override byte LightOpacity => 255;

		public override string DisplayName => "Lava";

		protected override bool AllowSourceCreation => false;

		protected override double SecondsBetweenUpdates => 2;

		private byte _MaximumFluidDepletion { get; }
		protected override byte MaximumFluidDepletion => _MaximumFluidDepletion;

		protected override byte FlowingID => BlockId;

		protected override byte StillID => StationaryLavaBlock.BlockId;
	}
}