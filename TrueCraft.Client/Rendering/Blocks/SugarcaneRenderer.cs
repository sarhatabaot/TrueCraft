using Microsoft.Xna.Framework;
using TrueCraft._ADDON.Blocks;

namespace TrueCraft.Client.Rendering.Blocks
{
	public class SugarcaneRenderer : FlatQuadRenderer
	{
		static SugarcaneRenderer() => RegisterRenderer(SugarcaneBlock.BlockId, new SugarcaneRenderer());

		protected override Vector2 TextureMap => new Vector2(9, 4);
	}
}