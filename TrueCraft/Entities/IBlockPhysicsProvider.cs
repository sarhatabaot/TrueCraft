using Microsoft.Xna.Framework;
using TrueCraft.World;

namespace TrueCraft.Entities
{
	public interface IBlockPhysicsProvider
	{
		BoundingBox? GetBoundingBox(IWorld world, Coordinates3D coordinates);
	}
}