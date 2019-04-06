using Microsoft.Xna.Framework;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.Client.Rendering.Blocks
{
	public class CobwebRenderer : FlatQuadRenderer
	{
		static CobwebRenderer() => BlockRenderer.RegisterRenderer(CobwebBlock.BlockId, new CobwebRenderer());

		protected override Vector2 TextureMap => new Vector2(11, 0);
	}
}