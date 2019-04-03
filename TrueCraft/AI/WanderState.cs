using System.Threading.Tasks;
using TrueCraft.Entities;
using TrueCraft.Server;
using TrueCraft.World;

namespace TrueCraft.AI
{
	public class WanderState : IMobState
	{
		public WanderState()
		{
			Distance = 25;
			PathFinder = new AStarPathFinder();
		}

		/// <summary>
		///  The maximum distance the mob will move in an iteration.
		/// </summary>
		/// <value>The distance.</value>
		public int Distance { get; set; }

		public AStarPathFinder PathFinder { get; set; }

		public void Update(IMobEntity entity, IEntityManager manager)
		{
			var cast = entity as IEntity;
			if (entity.CurrentPath != null)
			{
				if (entity.AdvancePath(manager.TimeSinceLastUpdate))
					entity.CurrentState = new IdleState(new WanderState());
			}
			else
			{
				var target = new Coordinates3D(
					(int) (cast.Position.X + (MathHelper.Random.Next(Distance) - Distance / 2)),
					0,
					(int) (cast.Position.Z + (MathHelper.Random.Next(Distance) - Distance / 2))
				);
				IChunk chunk;
				var adjusted = entity.World.FindBlockPosition(target, out chunk, false);
				target.Y = chunk.GetHeight((byte) adjusted.X, (byte) adjusted.Z) + 1;
				Task.Factory.StartNew(() =>
				{
					entity.CurrentPath = PathFinder.FindPath(entity.World, entity.BoundingBox,
						(Coordinates3D) cast.Position, target);
				});
			}
		}
	}
}