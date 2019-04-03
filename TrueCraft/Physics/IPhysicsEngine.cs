using System;
using TrueCraft.World;

namespace TrueCraft.Physics
{
	public interface IPhysicsEngine
	{
		IWorld World { get; set; }
		void AddEntity(IPhysicsEntity entity);
		void RemoveEntity(IPhysicsEntity entity);
		void Update(TimeSpan time);
	}
}