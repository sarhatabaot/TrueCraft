﻿using TrueCraft.Entities;

namespace TrueCraft.Entity
{
	public class WolfEntity : MobEntity
	{
		public override Size Size => new Size(0.6, 1.8, 0.6);

		public override short MaxHealth => 10;

		public override sbyte MobType => 95;
	}
}