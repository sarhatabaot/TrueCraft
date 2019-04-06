using TrueCraft.Entities;

namespace TrueCraft.Entity
{
	public class SheepEntity : MobEntity
	{
		public override Size Size => new Size(0.9, 1.3, 0.9);

		public override short MaxHealth => 8;

		public override sbyte MobType => 91;
	}
}