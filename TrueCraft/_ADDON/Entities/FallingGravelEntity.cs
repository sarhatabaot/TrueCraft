using Microsoft.Xna.Framework;

namespace TrueCraft.Entities
{
	public class FallingGravelEntity : FallingSandEntity
	{
		public FallingGravelEntity(Vector3 position) : base(position)
		{
		}

		public override byte EntityType => 71;
	}
}