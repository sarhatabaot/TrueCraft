using Microsoft.Xna.Framework;
using TrueCraft.API.World;

namespace TrueCraft.API.Entities
{
	public interface IBlockPhysicsProvider
	{
		BoundingBox? GetBoundingBox(IWorld world, Coordinates3D coordinates);
	}
}