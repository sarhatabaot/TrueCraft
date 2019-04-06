using TrueCraft.Entities;

namespace TrueCraft._ADDON.Entities
{
	public class ZombieEntity : MobEntity
	{
		public override Size Size => new Size(0.6, 1.8, 0.6);

		public override short MaxHealth => 20;

		public override sbyte MobType => 54;

		public override bool Friendly => false;
	}
}