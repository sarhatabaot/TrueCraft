using Microsoft.Xna.Framework;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Client.Rendering.Blocks
{
	public class CobwebRenderer : FlatQuadRenderer
	{
		static CobwebRenderer() => BlockRenderer.RegisterRenderer(CobwebBlock.BlockID, new CobwebRenderer());

		protected override Vector2 TextureMap => new Vector2(11, 0);
	}
}