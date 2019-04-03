using System;
using Microsoft.Xna.Framework;
using TrueCraft.AI;
using TrueCraft.Physics;

namespace TrueCraft.Entities
{
	public interface IMobEntity : IEntity, IAABBEntity
	{
		PathResult CurrentPath { get; set; }
		IMobState CurrentState { get; set; }
		event EventHandler PathComplete;
		bool AdvancePath(TimeSpan time, bool faceRoute = true);
		void Face(Vector3 target);
	}
}