using TrueCraft.Entities;

namespace TrueCraft.Entity
{
	public class SpiderEntity : MobEntity
	{
		public override Size Size => new Size(1.4, 0.9, 1.4);

		public override short MaxHealth => 16;

		public override sbyte MobType => 52;

		public override bool Friendly => false;
	}
}