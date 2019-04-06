using Microsoft.Xna.Framework;
using TrueCraft.Logic.Blocks;

namespace TrueCraft.Client.Rendering.Blocks
{
	public class SugarcaneRenderer : FlatQuadRenderer
	{
		static SugarcaneRenderer() => BlockRenderer.RegisterRenderer(SugarcaneBlock.BlockId, new SugarcaneRenderer());

		protected override Vector2 TextureMap => new Vector2(9, 4);
	}
}