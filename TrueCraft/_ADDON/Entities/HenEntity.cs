namespace TrueCraft.Entities
{
	public class HenEntity : MobEntity
	{
		public override Size Size => new Size(0.4, 0.3, 0.4);

		public override short MaxHealth => 4;

		public override sbyte MobType => 93;
	}
}