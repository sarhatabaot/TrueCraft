namespace TrueCraft.Logic.Blocks
{
	public abstract class MushroomBlock : BlockProvider
	{
		public override double BlastResistance => 0;

		public override double Hardness => 0;

		public override SoundEffectClass SoundEffect => SoundEffectClass.Grass;
	}
}