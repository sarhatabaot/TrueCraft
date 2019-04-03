using System;

namespace TrueCraft.Entities
{
	public class EntityEventArgs : EventArgs
	{
		public EntityEventArgs(IEntity entity) => Entity = entity;

		public IEntity Entity { get; set; }
	}
}