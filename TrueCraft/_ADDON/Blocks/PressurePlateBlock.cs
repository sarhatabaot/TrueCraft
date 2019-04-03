namespace TrueCraft.Logic.Blocks
{
	public abstract class PressurePlateBlock : BlockProvider
	{
		public override double BlastResistance => 2.5;

		public override double Hardness => 0.5;

		public override byte Luminance => 0;

		public override bool Opaque => false;
	}
}