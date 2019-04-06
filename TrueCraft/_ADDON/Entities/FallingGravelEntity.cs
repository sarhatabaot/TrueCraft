using Microsoft.Xna.Framework;

namespace TrueCraft._ADDON.Entities
{
	public class FallingGravelEntity : FallingSandEntity
	{
		public FallingGravelEntity(Vector3 position) : base(position)
		{
		}

		public override byte EntityType => 71;
	}
}